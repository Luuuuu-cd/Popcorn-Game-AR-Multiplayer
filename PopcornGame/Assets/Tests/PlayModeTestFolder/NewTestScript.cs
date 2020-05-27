using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator SinglePlayerButtonClickDirectsToMainScene()
        {
            ClickButton("SinglePlayerButton", "StartScene");
            AssertSceneLoaded("MainScene");
            yield return null;
       
        }

       

        [UnityTest]
        public IEnumerator  MultiplayerButtonClickDirectsToMultiplayerStartScene()
        {
            ClickButton("MultiplayerButton", "StartScene");
            AssertSceneLoaded("MultiplayerStartScene");
            yield return null;

        }

        [UnityTest]
        public IEnumerator GuestLoginButtonDirectsToStartScene()
        {
            ClickButton("ContinueAsGuest", "LoginScreen");
            AssertSceneLoaded("StartScene");
            yield return null;

            
        }

        [UnityTest]
        public IEnumerator ProfileButtonClickWhenNotSignedInDirectsToLoginScreen()
        {
            ClickButton("UserProfileButton", "StartScene");
            AssertSceneLoaded("LoginScreen");
            yield return null;

         
        }


        [UnityTest]
        public IEnumerator RestartButtonTakesBackToStartScene()
        {
            ClickButton("RestartButton", "GameOverScene");
            AssertSceneLoaded("StartScene");
            yield return null;

          
        }

        [UnityTest]
        public IEnumerator CreateRoomButtonDirectsToCreateRoomScene()
        {
            ClickButton("CreateButton", "MultiplayerStartScene");
            AssertSceneLoaded("Create room");
            yield return null;

          
        }


        [UnityTest]
        public IEnumerator JoinRoomButtonDirectsToJoinRoomScene()
        {
            ClickButton("JoinButton", "MultiplayerStartScene");
            AssertSceneLoaded("Join room");
            yield return null;

        }


        [UnityTest]
        public IEnumerator BackButtonInMultiplayerStartSceneDirectsToStartScene()
        {
            ClickButton("BackButton", "MultiplayerStartScene");
            AssertSceneLoaded("StartScene");
            yield return null;

           
        }

        [UnityTest]
        public IEnumerator BackButtonInJoinRoomSceneDirectsToMultiplayerStartScene()
        {
            ClickButton("BackButton", "Join room");
            AssertSceneLoaded("MultiplayerStartScene");
            yield return null;

       
        }

        [UnityTest]
        public IEnumerator BackButtonInCreateRoomSceneDirectsToMultiplayerStartScene()
        {
            ClickButton("BackButton", "Create room");
            AssertSceneLoaded("MultiplayerStartScene");
            yield return null;

           
        }
        [UnityTest]
        public IEnumerator StartButtonInWaitingRoomDirectsToMainScene()
        {
            ClickButton("StartGame Button", "WaitingRoom");
            AssertSceneLoaded("MainScene");
            yield return null;

            
        }

        [UnityTest]
        public IEnumerator BackButtonInUserPrefSceneDirectsToStartScene()
        {
            ClickButton("BackButton", "UserPersonalAccountPage");
            AssertSceneLoaded("StartScene");
            yield return null;


        }

        [UnityTest]
        public IEnumerator PopcornChoiceButtonDirectsToPopcornChoiceScene()
        {
            ClickButton("PopcornChoiceButton", "UserPersonalAccountPage");
            AssertSceneLoaded("PopcornChoice");
            yield return null;


        }

        [UnityTest]
        public IEnumerator BackButtonInPopcornChoiceSceneButtonDirectsToUserPref()
        {
            ClickButton("BackButton", "PopcornChoice");
            AssertSceneLoaded("UserPersonalAccountPage");
            yield return null;


        }


        public static IEnumerator AssertSceneLoaded(string sceneName)
        {
            var waitForScene = new WaitForSceneLoaded(sceneName);
            yield return waitForScene;
            Assert.IsFalse(waitForScene.TimedOut, "Scene " + sceneName + " was never loaded");
        }

        //
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        // testing if the single player game goes to the play screen 

        // testing if the munltiplayer button goes to the add players kinds scene 

        // helper method to similate a button click from the user 
        public IEnumerator ClickButton(string name, string sceneToLoad) {
            // finding button game object
            SceneManager.LoadScene(sceneToLoad);
            yield return null;
            var go = GameObject.Find(name);
            Assert.IsNotNull(go, "MIss button " + name);

            // set it selected for the event system 
            EventSystem.current.SetSelectedGameObject(go);

            // Invoke Click
            go.GetComponent<Button>().onClick.Invoke(); 

        }
        
    }
}
