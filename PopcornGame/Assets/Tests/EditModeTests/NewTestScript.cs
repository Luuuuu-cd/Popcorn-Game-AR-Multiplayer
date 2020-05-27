using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        // game-flow tests 

            // maybe do this in units 
            // unit 1: login screen stuff //  lets not start with this
            // unit 2: AR stuff -- Cannot emulate AR core functionality 
            // Game mechanics -- cannot emulate AR core functionality in unit tests because they require an Android device's camera 
        //and do not work with an emulator. 

        // LOBBY TestSUITE
        // A Test behaves as an ordinary method
        // Potential tests
        // Whenevr a user logins - he is added to the database 
        // Whenenever a user creates a room, a room is actually created by photon
        // Whenever a user joins a room, they join the right room 
        // The list of users in the room is consistent with the number of users who have clicked join 

        // AR CORE TESTS - NOT SURE IF POSSIBLE 
        // Whenever the place button works -> and a cloud anchor is created 
        // Other users are able to detect the same cloud anchor position and spawn on their screen 
        // Popcorn is clicked and user score goes up 
        // Fan button is on whenever the fan is eaten 
        // Ink button is on whenever the ink is eaten 
        
       
        // final score is consistent with the number of clicks 
        // 

        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
