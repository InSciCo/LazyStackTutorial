using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 

using LazyStackAuth;
using PetStoreClientSDK;

namespace PetStoreTests
{
    [TestClass]
    public class PetStoreIntegrationTest
    {

        [TestMethod]
        public async Task TestMethod1()
        {
            using var authMessagesStream = typeof(IAuthProcess).Assembly.GetManifestResourceStream("LazyStackAuth.AuthMessages.json");
            using var awsSettingsStream = typeof(PetStoreIntegrationTest).Assembly.GetManifestResourceStream("PetStoreTests.AwsSettings.json");
            using var methodMapStream = typeof(PetStore).Assembly.GetManifestResourceStream("PetStoreClientSDK.MethodMap.json");
            IConfiguration appConfig = new ConfigurationBuilder()
                .AddUserSecrets<PetStoreIntegrationTest>() //  used to get gmail account credentials for auth code
                .AddJsonStream(awsSettingsStream)
                .AddJsonStream(authMessagesStream)
                .AddJsonStream(methodMapStream)
                .Build();

            IServiceCollection services = new ServiceCollection();
            IServiceProvider serviceProvider;

            services.AddSingleton<IConfiguration>(appConfig);
            services.AddSingleton<IAuthProvider, AuthProviderCognito>(); // depends on IConfiguration
            services.AddSingleton<IAuthProcess, AuthProcess>(); // depends on IConfiguration, IAuthProvider

            serviceProvider = services.BuildServiceProvider();

            var authProvider = serviceProvider.GetService<IAuthProvider>() as AuthProviderCognito;
            var authProcess = serviceProvider.GetService<IAuthProcess>();

            // Signup
            var now = DateTime.Now;
            var login = $"TestUser{now.Year}-{now.Month}-{now.Day}-{now.Hour}-{now.Minute}-{now.Second}";
            var password = "TestUser1!";

            // Use Google email alias feature to allow us to use a single gmail account for our testing
            // ex: me999@gmai.com becomes me999+2020-10-20-25-96@gmail.com
            // This avoids AWS complaining that the email already exists
            // These are one-time use aliases and Cognito accounts
            var email = appConfig["Gmail:Email"];
            var emailParts = email.Split("@");
            email = emailParts[0] + "+" + login + "@" + emailParts[1];

            // Start Sign Up process
            Assert.IsTrue(await authProcess.StartSignUpAsync() == AuthEventEnum.AuthChallenge);
            Assert.IsTrue(authProcess.CurrentAuthProcess == AuthProcessEnum.SigningUp);

            // Simulate collection of Login from user
            Assert.IsTrue(authProcess.CurrentChallenge == AuthChallengeEnum.Login);
            authProcess.Login = login;
            Assert.IsTrue(await authProcess.VerifyLoginAsync() == AuthEventEnum.AuthChallenge);

            // Simulate collection of Password from user
            Assert.IsTrue(authProcess.CurrentChallenge == AuthChallengeEnum.Password);
            authProcess.Password = password;
            Assert.IsTrue(await authProcess.VerifyPasswordAsync() == AuthEventEnum.AuthChallenge);

            // Simulate collection of Email from user
            Assert.IsTrue(authProcess.CurrentChallenge == AuthChallengeEnum.Email);
            authProcess.Email = email;
            var verificationCodeSendTime = DateTime.UtcNow; // verificationCode sent after this time
            Thread.Sleep(5000); // Account for a little drive among local and remote clock
            Assert.IsTrue(await authProcess.VerifyEmailAsync() == AuthEventEnum.AuthChallenge);

            // Simulate collection of Code from user - AWS Sends the code to the email account
            Assert.IsTrue(authProcess.CurrentChallenge == AuthChallengeEnum.Code);
            var verificationCode = AuthEmail.GetAuthCode(appConfig, verificationCodeSendTime, email);
            Assert.IsNotNull(verificationCode);
            authProcess.Code = verificationCode;
            // Verifying the code finalizes the sign up process
            Assert.IsTrue(await authProcess.VerifyCodeAsync() == AuthEventEnum.SignedUp);
            Assert.IsTrue(authProcess.CurrentAuthProcess == AuthProcessEnum.None);
            Assert.IsTrue(authProcess.CurrentChallenge == AuthChallengeEnum.None);
            Assert.IsTrue(authProcess.IsSignedIn == false);

            // Start SignIn process
            Assert.IsTrue(await authProcess.StartSignInAsync() == AuthEventEnum.AuthChallenge);
            Assert.IsTrue(authProcess.CurrentAuthProcess == AuthProcessEnum.SigningIn);

            // Simulate collection of Password from user
            Assert.IsTrue(authProcess.CurrentChallenge == AuthChallengeEnum.Login);
            authProcess.Password = password;
            Assert.IsTrue(await authProcess.VerifyLoginAsync() == AuthEventEnum.AuthChallenge);

            // Simulate collection of Password from user
            Assert.IsTrue(authProcess.CurrentChallenge == AuthChallengeEnum.Password);
            authProcess.Password = password;
            // Verifying password finalizes SigningIn process
            Assert.IsTrue(await authProcess.VerifyPasswordAsync() == AuthEventEnum.SignedIn);
            Assert.IsTrue(authProcess.CurrentAuthProcess == AuthProcessEnum.None);
            Assert.IsTrue(authProcess.CurrentChallenge == AuthChallengeEnum.None);
            Assert.IsTrue(authProcess.IsSignedIn == true);

            //////////////////////////////////////////////////////////////
            // Use PetStore library to call the ApiGateways in our stack
            //////////////////////////////////////////////////////////////
            
            // Creating the LzHttpClient which knows how to make secure calls against AWS ApiGateways
            var lzHttpClient = new LzHttpClient(appConfig, authProvider, null);

            // Create the PetStore lib instance that makes calling the stack easy
            var petStore = new PetStore(lzHttpClient);

            // Call PetStore.SeedPetsAsync -- it's ok if the pet records already exist
            await petStore.SeedPetsAsync();

            // Retrieve pet record by Id
            var pet = await petStore.GetPetByIdAsync(1);


        }
    }
}
