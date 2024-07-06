using UnityEngine;

public class PowerUp : MonoBehaviour {
    public enum type {
        Magnet,
        Ghost,
        Slowdown,
        ScoreMultiplier
    }

    public Player_Powerup pp;

    void Start(){
        pp = Player_Powerup.instace;
    }

    public type _type;
    void OnTriggerEnter2D (Collider2D coll) {
        if (coll.tag == "Player") {
            switch (_type) {
                case type.Magnet:
                    pp.StartMagnet();
                    break;
                case type.Slowdown:
                    pp.StartSlowdown();
                    break;
                case type.Ghost:
                    pp.StartGhost();
                    break;
                case type.ScoreMultiplier:
                    pp.StartScoreMultiplier();
                    break;
            }
            SoundManager.instance.PlayPowerUpSound();
            //Vibration.Vibrate(30);
            gameObject.SetActive(false);
        }
    }
}