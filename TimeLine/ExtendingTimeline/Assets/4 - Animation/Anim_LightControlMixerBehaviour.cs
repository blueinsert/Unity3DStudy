using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AnimationExample
{
	public class Anim_LightControlMixerBehaviour : PlayableBehaviour
	{
		// NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			Light trackBinding = playerData as Light;
			float finalIntensity = 0f;
			Color finalColor = Color.black;

			if (!trackBinding)
				return;

			int inputCount = playable.GetInputCount (); //get the number of all clips on this track

			for (int i = 0; i < inputCount; i++)
			{
				float inputWeight = playable.GetInputWeight(i);
				ScriptPlayable<Anim_LightControlBehaviour> inputPlayable = (ScriptPlayable<Anim_LightControlBehaviour>)playable.GetInput(i);
				Anim_LightControlBehaviour input = inputPlayable.GetBehaviour();
				
				// Use the above variables to process each frame of this playable.
				finalIntensity += input.intensity * inputWeight;
				finalColor += input.color * inputWeight;
			}

			//assign the result to the bound object
			trackBinding.intensity = finalIntensity;
			trackBinding.color = finalColor;
		}
	}
}