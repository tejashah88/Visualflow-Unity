using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leap;
using Leap.Unity;
using Leap.Unity.Xtra;


public class HandController : TwoHandInterface {
  private enum MainViewState {
    COUNTRY,
    REGION,
  }

  public GameObject movable;

  bool cameraMoving;
  int frameCount;

  Vector3 currentPosition;

  MainViewState mainState, transToState;

	public void Start() {
    mainState = MainViewState.COUNTRY;
  }

  public override void Two(Hand[] presentHands) {
    float distance = Leap.Unity.Xtra.HandUtils.distanceSqrMagnitude(presentHands[0], presentHands[1]);
    float[] grabs = new float[] {
      Leap.Unity.Xtra.HandUtils.getGrabAngleDegrees(presentHands[0]),
      Leap.Unity.Xtra.HandUtils.getGrabAngleDegrees(presentHands[1])
    };
    Debug.Log(distance + "," + grabs[0]);

    MainViewState prevState = mainState;

    if (grabs[0] >= 100f && grabs[1] >= 100f) {
      if (distance >= 0.25f) {
        mainState = MainViewState.REGION;
      } else if (distance <= 0.05f) {
        mainState = MainViewState.COUNTRY;
      }
    }

    if (prevState != mainState) {
      cameraMoving = true;
      transToState = mainState;
      currentPosition = movable.transform.position;
    }

    if (cameraMoving) {
      frameCount += 1;
      Vector3 newPosition;
      if (transToState == MainViewState.REGION) {
        newPosition = new Vector3(0.8f, 17f, 21f);
      } else {
        newPosition = new Vector3(0, 22.5f, 33.5f);
      }
      movable.transform.position = Vector3.Lerp(currentPosition, newPosition, frameCount / 30.0f);

      if (frameCount >= 30) {
        frameCount = 0;
        cameraMoving = false;
        movable.transform.position = newPosition;
      }
    }
  }
}
