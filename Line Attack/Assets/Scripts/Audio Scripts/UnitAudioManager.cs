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

	public AudioSource audioSource;

	void Start()
    {
		audioSource = GetComponent<AudioSource>();
	}

	public void DrawSwordSound()
	{
		audioSource.PlayOneShot(drawSwordClip);
	}

	public void SwordCollideSound()
	{
		audioSource.PlayOneShot(swordCollideClip);
	}

	public void DrawFireSound()
	{
		audioSource.PlayOneShot(drawFireClip);
	}

	public void CasteSpell()
	{
		audioSource.PlayOneShot(casteSpellClip);
	}
}
