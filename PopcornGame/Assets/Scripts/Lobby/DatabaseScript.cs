using System.Collections.Generic;
using Proyecto26;
using FullSerializer;
using UnityEngine;
using System;

public class DatabaseScript : MonoBehaviour
{
    public User user { get; set; }
    public static int playerScore { get; set; }

    public static string userID; 
    public static string displayName;
    public static fsSerializer serializer = new fsSerializer();

    public void initialize(String name, String id) {
        userID = id;
        displayName = name; 
    }



    public void putUserToDatabase() {
        User u = new User(displayName, userID);
        RestClient.Put("https://popcorngameucl.firebaseio.com/" + userID + ".json", u);
    }

    public void updateUserScoreToDatabase(int score) {

        User u = new User(displayName, userID);
        u.userHighestScore = score; 
        RestClient.Put("https://popcorngameucl.firebaseio.com/" + userID + ".json", u);

    }


    public void addToDatabase() {
        //testing = "In checkDatabaseAndentry()";
        RestClient.Get("https://popcorngameucl.firebaseio.com/" + ".json").Then(response =>
        {
            fsData userData = fsJsonParser.Parse(response.Text);
            Dictionary<string, User> users = null;
            serializer.TryDeserialize(userData, ref users);
            bool isInDatabase = false;
            foreach (var oneUser in users.Values)
            {
                if (oneUser.userID.Equals(userID))
                {
                    isInDatabase = true;
                    playerScore = oneUser.userHighestScore;
                    break;
                }
                
            }
            if (!isInDatabase)
            {
                putUserToDatabase();
                playerScore = 0;
            }       
        }
        );

    }
}
