using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NiobiumStudios;
public class DailyRewardManager : MonoBehaviour
{
    public GameManager gm;
    void Awake()
    {
        DailyRewards.instance.onClaimPrize += OnClaimPrizeDailyRewards;
    }

    // this is your integration function. Can be on Start or simply a function to be called
    public void OnClaimPrizeDailyRewards(int day)
    {
       //This returns a Reward object
		Reward myReward = DailyRewards.instance.GetReward(day);

        // And you can access any property
        Debug.Log(myReward.unit);   // This is your reward Unit name
        Debug.Log(myReward.reward); // This is your reward count

		/*var rewardsCount = PlayerPrefs.GetInt ("MY_REWARD_KEY", 0);
		rewardsCount += myReward.reward;

		PlayerPrefs.SetInt ("MY_REWARD_KEY", rewardsCount);
		PlayerPrefs.Save ();*/

        if(myReward.unit == "Coins"){
            gm.Coins += myReward.reward;
            //GameManager.instance.gs.Coins += myReward.reward;
            //MainMenu.instance.Save();
        }else{
            gm._totalPlayers[myReward.unit].unlocked = true;
            gm.SaveCharacters();

            gm.LoadCharacters();
        }
    }
}
