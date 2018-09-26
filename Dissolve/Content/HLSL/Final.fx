sampler tSampler;

float2 xScreenSize;

float xBoostFactor;
bool xUIBlur;

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 retVal = float4(0,0,0,0);
	float4 original = tex2D(tSampler, input.TexCoord);
	float2 pixelDist = float2(0,0);
	pixelDist.x = 1/xScreenSize.x;
	pixelDist.y = 1/xScreenSize.y;
	float2 newCoords;
	
	if(xUIBlur)
	{
		for(int i = -1; i <= 1; i++)
		{
			for(int j = -1;j <= 1; j++)
			{
				newCoords.x = input.TexCoord.x + (pixelDist.x * i *1.2);
				newCoords.y = input.TexCoord.y + (pixelDist.y * j * 1.2);
				retVal += tex2D(tSampler, newCoords);
			}
		}
		retVal /= 9;
		
		retVal.r = max(retVal.r, original.r);
		retVal.b = max(retVal.b, original.b);
		retVal.g = max(retVal.g, original.g);
	}
	else
	{
		retVal = tex2D(tSampler, input.TexCoord);
	}
	

	if(retVal.b > retVal.g && retVal.b > retVal.r)
	{
		retVal.b += 0.5 * xBoostFactor;
		retVal.g += 0.25 * xBoostFactor;
	}

    return retVal;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
