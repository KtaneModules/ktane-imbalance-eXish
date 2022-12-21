using System;
using System.Collections;
using Imbalance;
using UnityEngine;
using Random = UnityEngine.Random;

public class ImbalanceScript : MonoBehaviour
{
	private void Start()
	{
		_id = ++_idCounter;
		_chosenA = Random.Range(0, 128);
		_chosenB = Random.Range(0, 128);
		_textA.text = _chosenA.ToImbalance();
		_textB.text = _chosenB.ToImbalance();
		Debug.LogFormat("[Imbalance #{0}] The top screen is displaying: \"{1}\"", new object[]
		{
			_id,
			_textA.text
		});
		Debug.LogFormat("[Imbalance #{0}] The bottom screen is displaying: \"{1}\"", new object[]
		{
			_id,
			_textB.text
		});
		_solution = _chosenA * _chosenB;
		Debug.LogFormat("[Imbalance #{0}] The expected solution is: \"{1}\"", new object[]
		{
			_id,
			_solution
		});
		KMSelectable button = _button;
		button.OnInteract = (KMSelectable.OnInteractHandler)Delegate.Combine(button.OnInteract, new KMSelectable.OnInteractHandler(() => Press()));
		KMSelectable button2 = _button;
		button2.OnInteractEnded = (Action)Delegate.Combine(button2.OnInteractEnded, new Action(delegate()
		{
			// This hold check fixes an issue with single selectable controller support
			if (_held)
            {
				_held = false;
				StartCoroutine(Animate(false));
			}
		}));
	}

	private IEnumerator Animate(bool v)
	{
		if (v)
		{
			_audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, _button.transform);
		}
		else
		{
			_audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, _button.transform);
		}
		float time = Time.time;
		while (Time.time < time + 0.125f)
		{
			_cap.transform.localPosition = new Vector3(0f, (!v) ? (-0.55f + (Time.time - time) / 0.25f) : (-0.55f * (Time.time - time) / 0.125f), 0f);
			yield return null;
		}
		_cap.transform.localPosition = new Vector3(0f, (!v) ? 0f : -0.55f, 0f);
		yield break;
	}

	private void Update()
	{
		_buttonText.text = ((int)_info.GetTime() % 10).ToString();
	}

	private bool Press()
	{
		_held = true;
		StartCoroutine(Animate(true));
		if (_isSolved)
		{
			return false;
		}
		if (_textA.text.EndsWith(".«"))
		{
			_textA.text = _buttonText.text;
		}
		else
		{
			TextMesh textA = _textA;
			textA.text += _buttonText.text;
		}
		if (!_solution.ToString().StartsWith(_textA.text))
		{
			Debug.LogFormat("[Imbalance #{0}] You entered \"{1}\", which was incorrect.", new object[]
			{
				_id,
				_textA.text
			});
			_module.HandleStrike();
			_textA.text = _chosenA.ToImbalance();
		}
		if (_solution.ToString() == _textA.text)
		{
			Debug.LogFormat("[Imbalance #{0}] You entered \"{1}\", which was correct. Solved.", new object[]
			{
				_id,
				_textA.text
			});
			_module.HandlePass();
			_isSolved = true;
			StartCoroutine(FadeOut());
		}
		return false;
	}

	private IEnumerator FadeOut()
	{
		float time = Time.time;
		while (Time.time - time < 1f)
		{
			_textA.color = new Color(1f, 1f, 1f, 1f - (Time.time - time));
			_textB.color = new Color(1f, 1f, 1f, 1f - (Time.time - time));
			_buttonText.color = new Color(1f, 1f, 1f, 1f - (Time.time - time));
			yield return null;
		}
		_textA.color = new Color(1f, 1f, 1f, 0f);
		_textB.color = new Color(1f, 1f, 1f, 0f);
		_buttonText.color = new Color(1f, 1f, 1f, 0f);
		yield break;
	}

	[SerializeField]
	private TextMesh _textA;

	[SerializeField]
	private TextMesh _textB;

	[SerializeField]
	private TextMesh _buttonText;

	[SerializeField]
	private GameObject _cap;

	[SerializeField]
	private KMSelectable _button;

	[SerializeField]
	private KMBombModule _module;

	[SerializeField]
	private KMBombInfo _info;

	[SerializeField]
	private KMAudio _audio;

	private static int _idCounter;

	private int _chosenA;

	private int _chosenB;

	private int _solution;

	private int _id;

	private bool _held;

	private bool _isSolved;
}
