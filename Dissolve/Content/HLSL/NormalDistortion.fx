Texture xTexture;

sampler dSampler;

sampler TextureSampler = sampler_state { 
		texture = <xTexture> ; 
		magfilter = LINEAR;
		minfilter = LINEAR;
		mipfilter=LINEAR; 
		AddressU = CLAMP; 
		AddressV = CLAMP;
		};



float xNormalStrength;

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 bumpColor = tex2D(TextureSampler, input.TexCoord);
	float2 newCoords = input.TexCoord;
	newCoords += xNormalStrength * (bumpColor.rg-0.5f)*2.0f;
	
	float4 retVal = tex2D(dSampler, newCoords);
	if(bumpColor.b < 0.8 && (bumpColor.r > 0.51 || bumpColor.g > 0.51))
	{
		retVal.gb += (bumpColor.r - 0.5 + bumpColor.g - 0.5)/1.2;
	}

    return retVal;
}

technique Technique1
{
    pass Pass0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
