#define PI 3.1415
#define MAX_ARRAY_COUNT 100

int _RingDistortionHitArrayCount;
float4 _RingDistortionHitArray[MAX_ARRAY_COUNT];
float _RingDistortionHitTimeArray[MAX_ARRAY_COUNT];

float RingFactor(float _distance, float _phase, float _period, float _width)
{
	float sinusFactor = cos(_distance * 2 * PI / _period - _phase);
	return sinusFactor;
}

float AttenuationFactor(float _distance)
{
	return exp(-(_distance * _distance * 9) / 2);
}

void RingFactor_float(float2 _uv, float2 _center, float _phase, float _period, float _width, out float _factor)
{
	float distance = length(_uv - _center);
	_factor = RingFactor(distance, _phase, _period, _width);
}

void RingFactorArray_float(float2 _uv, float _phase, float _period, float _width, float _vanishDistance, out float _factor)
{
	_factor = 0;
	float distance, attenuation, periodModifier;
	for(int i = 0; i < _RingDistortionHitArrayCount; i++)
	{
		distance = min(_vanishDistance, length(_uv - _RingDistortionHitArray[i]));
		attenuation = AttenuationFactor(distance / _vanishDistance);
		periodModifier = 1;
		_factor += attenuation * RingFactor(distance, _phase, periodModifier * _period, periodModifier * _width);
	}
}

float RingModifier(float _distance, float _maxAmplitude, float _phase, float _period, float _width, float _vanishDistance, float _timeSinceHit)
{
	// attenuation factor centered around a "main wave" pulse
	float departureOffset = _width / 2;
	float mainWavePosition = _timeSinceHit - departureOffset;
	float widthFactor = 2 / (_width * (1 + mainWavePosition));
	float hitTimeModifier = AttenuationFactor(widthFactor * (_distance - mainWavePosition));

	float attenuation = AttenuationFactor(_distance / _vanishDistance);
	float amplitude = attenuation * _maxAmplitude;

	float ringFactor = RingFactor(_distance, _phase, _period, _width);
	return hitTimeModifier * amplitude * ringFactor;
}

float RingModifierDerivative(float _distance, float _step, float _maxAmplitude, float _phase, float _period, float _width, float _vanishDistance, float _timeSinceHit)
{
	float ringFactor1 = RingModifier(_distance, _maxAmplitude, _phase, _period, _width, _vanishDistance, _timeSinceHit);
	float ringFactor2 = RingModifier(_distance + _step, _maxAmplitude, _phase, _period, _width, _vanishDistance, _timeSinceHit);
	return (ringFactor2 - ringFactor1) / _step;
}

void RingFactor3DArray_float(float3 _inPos, float3 _inNormal, float _maxAmplitude, float _phase, float _period, float _width, float _vanishDistance, out float3 _outPos, out float3 _outNormal)
{
	_outPos = _inPos;
	_outNormal = _inNormal;
	float3 displacement = float3(0, 0, 0);
	float3 normalComponent = float3(0, 0, 0);
	float step = 0.01;
	for(int i = 0; i <  _RingDistortionHitArrayCount; i++)
	{
	    float4 hitCenter = _RingDistortionHitArray[i];
		float distance = length(_inPos - hitCenter);

		float hitTime = _RingDistortionHitTimeArray[i];
		float timeSinceHit = _Time.y - hitTime;

		float ringModifier = RingModifier(distance, _maxAmplitude, _phase, _period, _width, _vanishDistance, timeSinceHit);
		float ringModifierDerivative = RingModifierDerivative(distance, step, _maxAmplitude, _phase, _period, _width, _vanishDistance, timeSinceHit);

		float3 hitCenterToPos = _inPos - hitCenter.xyz;
		float3 hitCenterToPosProjected = - normalize(hitCenterToPos - dot(hitCenterToPos, _inNormal) * _inNormal); 

		_outPos += _inNormal * ringModifier;
		normalComponent += hitCenterToPosProjected * ringModifierDerivative; 
	}
	_outNormal = normalComponent + _inNormal;
}