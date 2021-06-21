using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using LazyStackAuth;
using PetStoreClientSDK;
using PetStoreSchema.Models;

namespace PetStoreConsoleApp
{
    class Program
    {
        static IAuthProcess authProcess;
        static IConfiguration appConfig;
        static string currentLanguage = "en-US";

        static async Task Main(string[] args)
        {
            // Load Configuration Files
            using var authMessagesStream = typeof(IAuthProcess).Assembly.GetManifestResourceStream("LazyStackAuth.AuthMessages.json");
            using var methodMapStream = typeof(PetStore).Assembly.GetManifestResourceStream("PetStoreClientSDK.MethodMap.json");
            using var awsSettingsStream = typeof(Program).Assembly.GetManifestResourceStream("PetStoreConsoleApp.AwsSettings.json");
            using var localApisStream = typeof(Program).Assembly.GetManifestResourceStream("PetStoreConsoleApp.LocalApis.json");
            appConfig = new ConfigurationBuilder()
                .AddJsonStream(authMessagesStream)
                .AddJsonStream(methodMapStream)
                .AddJsonStream(awsSettingsStream)
                .AddJsonStream(localApisStream)
                .Build();

            // Init AuthProvider and AuthProcess
            var authProvider = new AuthProviderCognito(appConfig); // Implements IAuthProvider interface for AWS Cognito
            authProcess = new AuthProcess(appConfig, authProvider); // Implements IAuthProcess interface

            // Ask the user if they want to use LocalApi Controller Calls
            // Note that this does not influence calls to Cognito for authentication
            Console.WriteLine("Welcome to the PetStoreConsoleApp program");
            Console.Write("Use LocalApi? y/n:");
            var useLocalApi = Console.ReadLine();
            var localApi = useLocalApi.Equals("y", StringComparison.OrdinalIgnoreCase) ? "Local" : null; 

            // Init LzHttpClient -- A HttpClient that knows how to securly communication with AWS
            var lzHttpClient = new LzHttpClient(appConfig, authProvider, localApi); 

            // Init PetStore library -- provides easy to use calls against AWS ApiGateways
            var petStore = new PetStore(lzHttpClient); 

            var input = string.Empty;
            var alertMsg = string.Empty;
            var menuMsg = string.Empty;
            var lastAuthEvent = AuthEventEnum.AuthChallenge;
            while(true)
            {
                alertMsg = (lastAuthEvent > AuthEventEnum.Alert)
                    ? appConfig[$"AuthAlertMessages:{currentLanguage}:{lastAuthEvent}"]
                    : string.Empty;
                if (!string.IsNullOrEmpty(alertMsg))
                    Console.WriteLine($"Alert: {alertMsg}");


                switch (authProcess.CurrentAuthProcess)
                {
                    case AuthProcessEnum.None:
                        if (authProcess.IsNotSignedIn)
                        {
                            menuMsg = appConfig[$"AuthProcessLabels:{currentLanguage}:{AuthProcessEnum.SigningUp}"];
                            Console.WriteLine($"1. {menuMsg}");

                            menuMsg = appConfig[$"AuthProcessLabels:{currentLanguage}:{AuthProcessEnum.SigningIn}"];
                            Console.WriteLine($"2. {menuMsg}");

                            menuMsg = appConfig[$"AuthProcessLabels:{currentLanguage}:{AuthProcessEnum.ResettingPassword}"];
                            Console.WriteLine($"3. {menuMsg}");

                            Console.WriteLine("4. Quit");

                            Console.Write(">");
                            input = Console.ReadLine();
                            switch (input)
                            {
                                case "1":
                                    lastAuthEvent = await authProcess.StartSignUpAsync();
                                    break;
                                case "2":
                                    lastAuthEvent = await authProcess.StartSignInAsync();
                                    break;
                                case "3":
                                    lastAuthEvent = await authProcess.StartResetPasswordAsync();
                                    break;
                                case "4":
                                    return; // exit program
                                default:
                                    Console.WriteLine("Sorry, didn't understand that input. Please try again.");
                                    continue;
                            }
                        }
                        else
                        {
                            menuMsg = appConfig[$"AuthProcessLabels:{currentLanguage}:{AuthProcessEnum.SigningOut}"];
                            Console.WriteLine($"1. {menuMsg}");

                            menuMsg = appConfig[$"AuthProcessLabels:{currentLanguage}:{AuthProcessEnum.UpdatingPassword}"];
                            Console.WriteLine($"2. {menuMsg}");

                            menuMsg = appConfig[$"AuthProcessLabels:{currentLanguage}:{AuthProcessEnum.UpdatingPhone}"];
                            Console.WriteLine($"3. {menuMsg}");

                            menuMsg = appConfig[$"AuthProcessLabels:{currentLanguage}:{AuthProcessEnum.UpdatingLogin}"];
                            Console.WriteLine($"4. {menuMsg}");

                            Console.WriteLine("5. Seed Pets data");
                            Console.WriteLine("6. Get Pet 1");
                            Console.WriteLine("7. See Available Pets");
                            Console.WriteLine("8. Quit");
                            Console.Write(">");
                            input = Console.ReadLine();
                            switch(input)
                            {
                                case "1":
                                    lastAuthEvent = await authProcess.SignOutAsync();
                                    break;
                                case "2":
                                    lastAuthEvent = await authProcess.StartUpdatePasswordAsync();
                                    break;
                                case "3":
                                    lastAuthEvent = await authProcess.StartUpdatePhoneAsync();
                                    break;
                                case "4":
                                    lastAuthEvent = await authProcess.StartUpdateLoginAsync();
                                    break;
                                case "5":
                                    try
                                    {
                                        await petStore.SeedPetsAsync();
                                        Console.WriteLine("Pets added to database");
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine($"Could not add pets to database. {e.Message}");
                                    }
                                    break;
                                case "6":
                                    try
                                    {
                                        var pet = await petStore.GetPetByIdAsync(1);
                                        Console.WriteLine($"pet: {pet.Id} {pet.Name} {pet.PetStatus}");
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine($"Could not retrieve pet. {e.Message}");
                                    }
                                    break;
                                case "7":
                                    try
                                    {
                                        var petStatus = new List<PetStatus> { PetStatus.Available };
                                        var pets = await petStore.FindPetsByStatusAsync(petStatus);
                                        if (pets.Count == 0)
                                            Console.WriteLine("Sorry - No Pets Found");
                                        else 
                                            foreach (var pet in pets)
                                                Console.WriteLine($"- Id: {pet.Id} Name: {pet.Name}" );
                                        Console.WriteLine();
                                    } catch (Exception e)
                                    {
                                        Console.WriteLine($"Could not get pets. {e.Message}");
                                    }
                                    break;
                                case "8":
                                    return; // exit program
                                default:
                                    Console.WriteLine("Sorry, didn't understand that input. Please try again.");
                                    continue;
                            }
                        }
                        break;
                    default: // Some AuthProcess is active so process CurrentChallenge
                        var prompt = appConfig[$"AuthChallengeMessages:{currentLanguage}:{authProcess.CurrentChallenge}"];
                        switch(authProcess.CurrentChallenge)
                        {

                            case AuthChallengeEnum.Login:
                                Console.Write($"{prompt}: ");
                                authProcess.Login = Console.ReadLine();
                                lastAuthEvent = string.IsNullOrEmpty(authProcess.Login)
                                    ? await CancelAsync()
                                    : await authProcess.VerifyLoginAsync();
                                break;

                            case AuthChallengeEnum.Password:
                                Console.Write($"{prompt}: ");
                                authProcess.Password = Console.ReadLine();
                                lastAuthEvent = string.IsNullOrEmpty(authProcess.Password)
                                    ? await CancelAsync()
                                    : await authProcess.VerifyPasswordAsync();
                                break;

                            case AuthChallengeEnum.Email:
                                Console.Write($"{prompt}: ");
                                authProcess.Email = Console.ReadLine();
                                lastAuthEvent = string.IsNullOrEmpty(authProcess.Email)
                                    ? await CancelAsync()
                                    : await authProcess.VerifyEmailAsync();
                                break;

                            case AuthChallengeEnum.Code:
                                Console.Write($"{prompt}: ");
                                authProcess.Code = Console.ReadLine();
                                if (string.IsNullOrEmpty(authProcess.Code))
                                {
                                    Console.Write("Resend Code? y/n");
                                    var resendCode = Console.ReadLine();
                                    lastAuthEvent = resendCode.Equals("y", StringComparison.OrdinalIgnoreCase)
                                        ? await CancelAsync()
                                        : await authProcess.ResendCodeAsync();
                                }
                                else
                                    lastAuthEvent = await authProcess.VerifyCodeAsync();

                                if (lastAuthEvent == AuthEventEnum.SignedUp)
                                    Console.WriteLine("You are Signed Up! You can now SignIn.");
                                break;

                            case AuthChallengeEnum.Phone:
                                Console.Write($"{prompt}: ");
                                authProcess.Phone = Console.ReadLine();
                                lastAuthEvent = string.IsNullOrEmpty(authProcess.Phone)
                                    ? await CancelAsync()
                                    : await authProcess.VerifyPhoneAsync();
                                break;

                            case AuthChallengeEnum.NewLogin:
                                Console.Write($"{prompt}: ");
                                authProcess.NewLogin = Console.ReadLine();
                                lastAuthEvent = string.IsNullOrEmpty(authProcess.NewLogin)
                                    ? await CancelAsync()
                                    : await authProcess.VerifyNewLoginAsync();
                                break;

                            case AuthChallengeEnum.NewPassword:
                                Console.Write($"{prompt}: ");
                                authProcess.NewPassword = Console.ReadLine();
                                lastAuthEvent = string.IsNullOrEmpty(authProcess.NewPassword)
                                    ? await CancelAsync()
                                    : await authProcess.VerifyNewPasswordAsync();
                                break;

                            case AuthChallengeEnum.NewEmail:
                                Console.Write($"{prompt}: ");
                                authProcess.NewEmail = Console.ReadLine();
                                lastAuthEvent = string.IsNullOrEmpty(authProcess.NewEmail)
                                    ? await CancelAsync()
                                    : await authProcess.VerifyNewEmailAsync();
                                break;

                            case AuthChallengeEnum.NewPhone:
                                Console.Write($"{prompt}: ");
                                authProcess.NewPhone = Console.ReadLine();
                                lastAuthEvent = string.IsNullOrEmpty(authProcess.NewPhone)
                                    ? await CancelAsync()
                                    : await authProcess.VerifyNewPhoneAsync();
                                break;

                            default:
                                Console.WriteLine("Error: Unknown Challenge!");
                                lastAuthEvent = await CancelAsync();
                                break;
                        }
                        break;
                }   
            }
        }
    
        static async Task<AuthEventEnum> CancelAsync()
        {
            var msg = appConfig[$"AuthProcessMessages:{currentLanguage}:{authProcess.CurrentAuthProcess}"];
            Console.WriteLine($"Canceling {msg} process");
            return await authProcess.CancelAsync();
        }
    }

}
