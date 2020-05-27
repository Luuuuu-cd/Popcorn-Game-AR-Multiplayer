using System;
using Firebase;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSignInDemo : MonoBehaviour
{
    public Text infoText;
    public string webClientId = "270427355236-4fofs0sj0qu0ni6hil0ldha0dpcip4cf.apps.googleusercontent.com";
    public bool isAuthFailed = false;
    public bool isGuest=false;
    public static GoogleSignInDemo _instance;

    private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    private void Awake()
    {
        _instance = this;
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        CheckFirebaseDependencies();
    }

    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                    auth = FirebaseAuth.DefaultInstance;
            }
        });
    }

    public void SignInWithGoogle()
    {
        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
        OnSignIn();
    }
    public void SignOutFromGoogle() { OnSignOut(); }
    public void ContinueAsGuest()
    {
        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayButtonClickedSound();
        isGuest = true;
        SceneManager.LoadScene("StartScene");
    }

    private async void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        try
        {

            var signedIn = await GoogleSignIn.DefaultInstance.SignIn();
            if (signedIn == null) {
                isAuthFailed = true;
            }
            OnAuthenticationFinished(signedIn.DisplayName,signedIn.UserId);
        }
        catch (Exception ex)
        {
            // exception thrown when login fails
        }
    }

    private void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        SceneManager.LoadScene("LoginScreen");
        

    }

    public void OnDisconnect()
    {
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(string name, string id)
    {
     
        DatabaseScript database = new DatabaseScript();
        database.initialize(name, id);
        database.addToDatabase(); 
        SceneManager.LoadScene("StartScene");
    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
        });
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;
    }

    private void AddToInformation(string str) { infoText.text += str; }
}