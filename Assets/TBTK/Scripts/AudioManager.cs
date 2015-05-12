using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	public class AudioManager : MonoBehaviour {

		private List<AudioSource> audioSourceList=new List<AudioSource>();
		
		private static float musicVolume=1f;
		private static float sfxVolume=1f;
		
		public List<AudioClip> musicList;
		public bool playMusic=true;
		public bool shuffle=false;
		private int currentTrackID=0;
		private AudioSource musicSource;
		
		
		private static AudioManager instance;
		private GameObject thisObj;
		private Transform thisT;
		
		
		public static void Init(){
			if(instance!=null) return;
			GameObject obj=new GameObject();
			obj.name="AudioManager";
			obj.AddComponent<AudioManager>();
		}
		
		
		void Awake(){
			if(instance!=null){
				Destroy(gameObject);
				return;
			}
			
			instance=this;
			
			thisObj=gameObject;
			thisT=transform;
			
			DontDestroyOnLoad(thisObj);

			musicList = new List<AudioClip> ();

			AddMusic (Levels_DB.GetLevelMusic (MapController.level));

			if(playMusic && musicList!=null && musicList.Count>0){
				musicSource=thisObj.AddComponent<AudioSource>();
				musicSource.loop=true;
				musicSource.playOnAwake=true;
				musicSource.volume=musicVolume;
				
				musicSource.ignoreListenerVolume=true;


			}
			
			audioSourceList=new List<AudioSource>();
			for(int i=0; i<5; i++){
				GameObject obj=new GameObject();
				obj.name="AudioSource"+(i+1);
				
				AudioSource src=obj.AddComponent<AudioSource>();
				src.playOnAwake=false;
				src.loop=false;
				
				obj.transform.parent=thisT;
				obj.transform.localPosition=Vector3.zero;
				
				audioSourceList.Add(src);
			}
			
			AudioListener.volume=sfxVolume;
		}
		
		
		void Update(){
		
			if(musicSource!=null && !musicSource.isPlaying){
				if(shuffle) musicSource.clip=musicList[Random.Range(0, musicList.Count)];
				else{
					musicSource.clip=musicList[currentTrackID];
					currentTrackID+=1;
					if(currentTrackID==musicList.Count) currentTrackID=0;
				}
				
				musicSource.Play();
			}
		}
		
		
		void OnEnable(){
			GameControl.onGameOverE += OnGameOver;
			GameControl.onGameRestartE += RestartGame;
			GameControl.onGameStartE += OnLevelStart;
			
			AbilityManagerFaction.onAbilityActivatedE += OnAbilityActivated;
			AbilityManagerUnit.onAbilityActivatedE += OnAbilityActivated;

			
		}
		
		void OnDisable(){
			GameControl.onGameOverE -= OnGameOver;
			GameControl.onGameRestartE -= RestartGame;
			GameControl.onGameStartE -= OnLevelStart;

			
			AbilityManagerFaction.onAbilityActivatedE -= OnAbilityActivated;
			AbilityManagerUnit.onAbilityActivatedE -= OnAbilityActivated;

		}
		
		
		
		void OnGameOver(int factionID){ 
			bool playerWon=FactionManager.IsPlayerFaction(factionID);
			if(playerWon){ if(gameWonSound!=null) _PlaySound(gameWonSound);  }
			else{ if(gameLostSound!=null) _PlaySound(gameLostSound);  }
		}
		
		void OnAbilityActivated(){ if(abilityActivatedSound!=null) _PlaySound(abilityActivatedSound); }
		
		
		
		public AudioClip gameWonSound;
		public AudioClip gameLostSound;
		
		public AudioClip abilityActivatedSound;
		
		
		
		
		//check for the next free, unused audioObject
		private int GetUnusedAudioSourceID(){
			for(int i=0; i<audioSourceList.Count; i++){
				if(!audioSourceList[i].isPlaying) return i;
			}
			return 0;	//if everything is used up, use item number zero
		}
		
		
		//call to play a specific clip
		public static void PlaySound(AudioClip clip){ 
			if(instance==null) Init();
			instance._PlaySound(clip);
		}
		public void _PlaySound(AudioClip clip){ 
			int ID=GetUnusedAudioSourceID();
			
			audioSourceList[ID].clip=clip;
			audioSourceList[ID].Play();
		}
		
		
		
		
		
		
		public static void SetSFXVolume(float val){
			sfxVolume=val;
			AudioListener.volume=val;
		}
		
		public static void SetMusicVolume(float val){
			musicVolume=val;
			if(instance && instance.musicSource) instance.musicSource.volume=val;
		}

		private void AddMusic(AudioClip audio){
			if (instance.musicList.Count != 0 && !instance.musicList[0].Equals(audio)) {
				instance.musicSource.Stop();
				instance.musicList[0] = null;

				instance.musicList.Add(audio);
			}
			else if(instance.musicList.Count == 0 ) instance.musicList.Add(audio);

		}
		
		public static float GetMusicVolume(){ return musicVolume; }
		public static float GetSFXVolume(){ return sfxVolume; }

		public static void RestartGame(){
			instance.OnDisable ();
			Destroy (instance.gameObject);
		}


		public static void OnLevelStart(){
			instance.AddMusic (Levels_DB.GetLevelMusic (MapController.level));
		}

	}




}