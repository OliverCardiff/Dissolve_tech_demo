sampler tSampler ;//= sampler_state{ AddressU = WRAP; AddressV = WRAP; };

float2 xScreenSize;

bool xAllowAlpha;

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};



float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 outPos = float4(0,0,0,1);
	float2 pixelDist = float2(0,0);
	pixelDist.x = 1/xScreenSize.x;
	pixelDist.y = 1/xScreenSize.y;
	float2 newCoords;
	
	if(xAllowAlpha == false)
	{
		for(int i = -1; i <= 1; i++)
		{
			for(int j = -1;j <= 1; j++)
			{
				newCoords.x = input.TexCoord.x + (pixelDist.x * i);
				newCoords.y = input.TexCoord.y + (pixelDist.y * j);
				outPos += tex2D(tSampler, newCoords);
			}
		}
		if(outPos.b + outPos.g + outPos.r > 2)
		{
			outPos /= 9.04;
		}
		else
		{
			int which = 0;
			float max = outPos.r;
			if(max < outPos.g) { max = outPos.g; which = 1;}
			if(max < outPos.b) { max = outPos.b; which = 2;}
			
			if(which == 0)
			{
				outPos.gb = 0;
			}
			else if(which == 1)
			{
				outPos.r = 0;
			}
			else if(which == 2)
			{
				outPos.rg = 0;
			}
		}
	}
	else
	{
		outPos = tex2D(tSampler, input.TexCoord);
		if(outPos.r + outPos.b + outPos.g > 2.5)
		{
			outPos.a = 0;
		}
	}
    return outPos;
}

technique Technique1
{
    pass Pass0
    {

        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
