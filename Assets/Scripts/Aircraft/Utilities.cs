using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    
	/*
     * The MIT License (MIT)
     * 
     * Copyright (c) 2008 Daniel Brauer
     * 
     * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
     * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
     * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

     * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
     * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
     * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
     * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
     */

	//first-order intercept using absolute target position
	public static Vector3 FirstOrderIntercept(
		Vector3 shooterPosition,
		Vector3 shooterVelocity,
		float shotSpeed,
		Vector3 targetPosition,
		Vector3 targetVelocity
	) {
		Vector3 targetRelativePosition = targetPosition - shooterPosition;
		Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
		float t = FirstOrderInterceptTime(
			shotSpeed,
			targetRelativePosition,
			targetRelativeVelocity
		);
		return targetPosition + t * (targetRelativeVelocity);
	}

	//first-order intercept using relative target position
	public static float FirstOrderInterceptTime(
		float shotSpeed,
		Vector3 targetRelativePosition,
		Vector3 targetRelativeVelocity
	) {
		float velocitySquared = targetRelativeVelocity.sqrMagnitude;
		if (velocitySquared < 0.001f) {
			return 0f;
		}

		float a = velocitySquared - shotSpeed * shotSpeed;

		//handle similar velocities
		if (Mathf.Abs(a) < 0.001f) {
			float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
			return Mathf.Max(t, 0f); //don't shoot back in time
		}

		float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
		float c = targetRelativePosition.sqrMagnitude;
		float determinant = b * b - 4f * a * c;

		if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
			float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
					t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
			if (t1 > 0f) {
				if (t2 > 0f)
					return Mathf.Min(t1, t2); //both are positive
				else {
					return t1; //only t1 is positive
				}
			} else {
				return Mathf.Max(t2, 0f); //don't shoot back in time
			}
		} else if (determinant < 0f) { //determinant < 0; no intercept path
			return 0f;
		} else { //determinant = 0; one intercept path, pretty much never happens
			return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
		}
	}

    // Pergrine7 expand to include second order intercept
    public static Vector3 SecondOrderIntercept(
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity,
        Vector3 targetAcceleration,
        float estimatedAngularAcceleration
    ) {
        // Calculate first-order intercept
        Vector3 interceptPoint = FirstOrderIntercept(
            shooterPosition, shooterVelocity, shotSpeed, targetPosition, targetVelocity
        );
        // Estimate the time to intercept based on the first order calculation
        float interceptTime = FirstOrderInterceptTime(
            shotSpeed,
            targetPosition - shooterPosition,
            targetVelocity - shooterVelocity
        );

        Vector3 linearAccelerationOffset = 0.5f * interceptTime * interceptTime * targetAcceleration;

        // Step 2 and 3: Estimate and apply angular offset
        // This is a basic approximation and might need adjustment for better accuracy
        float angularOffsetMagnitude = 0.5f * estimatedAngularAcceleration * interceptTime * interceptTime;
        Vector3 angularOffsetDirection = Vector3.Cross(targetVelocity, targetAcceleration).normalized;
        Vector3 angularOffset = angularOffsetDirection * angularOffsetMagnitude;

        // Step 4: Combine offsets
        Vector3 secondOrderIntercept = interceptPoint + linearAccelerationOffset + angularOffset;

        return secondOrderIntercept;
    }

        public static Vector3 SecondOrderInterceptAdv(
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity,
        Vector3 targetAcceleration,
        float estimatedAngularAcceleration
    ) {
        // Calculate first-order intercept
        Vector3 interceptPoint = FirstOrderIntercept(
            shooterPosition, shooterVelocity, shotSpeed, targetPosition, targetVelocity
        );
        // Estimate the time to intercept based on the first order calculation
        float interceptTime = FirstOrderInterceptTime(
            shotSpeed,
            targetPosition - shooterPosition,
            targetVelocity - shooterVelocity
        );

        Vector3 linearAccelerationOffset = 0.5f * interceptTime * interceptTime * targetAcceleration;

        // Step 2 and 3: Estimate and apply angular offset
        // This is a basic approximation and might need adjustment for better accuracy
        float angularOffsetMagnitude = 0.5f * estimatedAngularAcceleration * interceptTime * interceptTime;
        Vector3 angularOffsetDirection = Vector3.Cross(targetVelocity, targetAcceleration).normalized;
        Vector3 angularOffset = angularOffsetDirection * angularOffsetMagnitude;

        // Step 4: Combine offsets
        Vector3 secondOrderIntercept = interceptPoint + linearAccelerationOffset + angularOffset;

        return secondOrderIntercept;
    }
}
