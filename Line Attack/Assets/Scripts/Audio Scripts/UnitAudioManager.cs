using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UnitAudioManager : MonoBehaviour
{
	// Start is called before the first frame update

	public AudioClip drawSwordClip;
	public AudioClip swordCollideClip;
	public AudioClip drawFireClip;
	public AudioClip casteSpellClip;

	AudioSource audioSource;

	void Start()
    {
		audioSource = GetComponent<AudioSource>();
	}

	public void DrawSwordSound()
	{

	}

	public void SwordCollideSound()
	{

	}

	public void DrawFireSound()
	{

	}

	public void CasteSpell()
	{

	}
}
