using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using PetStoreMobileApp.Services;
using PetStoreMobileApp.Views;
using PetStoreClientSDK;
using LazyStackAuth;


namespace PetStoreMobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            using var authMessagesStream = typeof(IAuthProcess).Assembly.GetManifestResourceStream("LazyStackAuth.AuthMessages.json");
            using var awsSettingsStream = typeof(App).Assembly.GetManifestResourceStream("PetStoreMobileApp.AwsSettings.json");
            using var methodMapStream = typeof(PetStore).Assembly.GetManifestResourceStream("PetStoreClientSDK.MethodMap.json");
            using var localApisStream = typeof(App).Assembly.GetManifestResourceStream("PetStoreMobileApp.LocalApis.json");
            var appConfig = new ConfigurationBuilder()
                .AddJsonStream(authMessagesStream)
                .AddJsonStream(awsSettingsStream)
                .AddJsonStream(methodMapStream)
                .AddJsonStream(localApisStream)
                .Build();

            DependencyService.RegisterSingleton<IConfiguration>(appConfig);

            // Init PetStoreClientAWS and PetStoreClientSDK assets
            var authProvider = new AuthProviderCognito(appConfig);

            DependencyService.RegisterSingleton<IAuthProcess>(new AuthProcess(appConfig, authProvider));

            var localApiName = false ? "LocalAndriod" : null;
            ILzHttpClient lzHttpClient = new LzHttpClient(appConfig, authProvider, localApiName);
            PetStore = new PetStore(lzHttpClient); // replace with registration when NSwag provides an interface

            MainPage = new AppShell();
        }

        public static PetStore PetStore; // Not using DI because there is no IPetStore (LazyStack uses NSwag client generator. NSwag is working on interface generation)


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
