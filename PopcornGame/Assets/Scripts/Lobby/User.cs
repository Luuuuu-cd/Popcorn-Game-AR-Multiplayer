[System.Serializable]
public class User
{
    public string userID = "";
    public string name = "";
    public int userHighestScore = 0;

    public User(string name, string userID){
        this.userID = userID;
        this.name = name; 
    }
    public string getName() {
        return name;

    }
    public void setUserHighestScore(int score) {
        this.userHighestScore = score; 
    }
    public int getHighestScore() {
        return this.userHighestScore; 
    }

    public override string ToString()
    {
        return "Person: " + name + " UserID" + userID + " "+ userHighestScore;
    }

}
