using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashData
{
	public struct Attacker
	{
	    public int gameStarted; //= Animator.StringToHash("isGameStarted");//attackerAnimator.SetBool("isGameStarted", true);
	    public int win; //attackerAnimator.SetBool("isAttackerWon", true);
	    public int winMirror; //attackerAnimator.SetBool("isAttackerWonMirror", true);
    }
   
	public struct Defender
	{
		public int run;
	    public int tackle;// animator.SetBool("isTackling", true);
	    public int tackleEnd;// animator.SetBool("isTackleEnded", true);
	    public int fall;
	    public int lost;// animator.SetBool("isLost", true);
	    public int win;//animator.SetBool("isWon", true);
    }

	public struct Watcher
	{
		public int clap;// anim.SetBool("Clapping", true);
	    public int stand;// anim.SetBool("Standing", true);
	    public int lift;// anim.SetTrigger("Lift");
	}

	public Attacker attackerHash;
	public Defender defnderHash;
	public Watcher watcherHash;
	
	public HashData()
	{
		attackerHash = new Attacker();
		attackerHash.gameStarted = Animator.StringToHash("isGameStarted");
		attackerHash.win = Animator.StringToHash("isAttackerWon");
		attackerHash.winMirror = Animator.StringToHash("isAttackerWonMirror");
		
		defnderHash = new Defender();
		defnderHash.run = Animator.StringToHash("isRunning");
		defnderHash.tackle = Animator.StringToHash("isTackling");
		defnderHash.tackleEnd = Animator.StringToHash("isTackleEnded");
		defnderHash.fall = Animator.StringToHash("Fall");
		defnderHash.lost = Animator.StringToHash("isLost");
		defnderHash.win = Animator.StringToHash("isWon");
		
		watcherHash = new Watcher();
		watcherHash.clap = Animator.StringToHash("Clapping");
		watcherHash.stand = Animator.StringToHash("Standing");
		watcherHash.lift = Animator.StringToHash("Lift");
	}
}
