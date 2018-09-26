float xTime;
float xXScreenDimension;
float xYScreenDimension;
float xDuration;
bool xSample;

float4 xStartColor;
float4 xEndColor;

Texture xTexture;

sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR;
    minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 Velocity :TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    
	output.Position.x = input.Position.x + ((input.Velocity.x * 1.6 * xTime)/xXScreenDimension);
	output.Position.y = input.Position.y + ((input.Velocity.y  * 0.3 * xTime)/xYScreenDimension);
	output.Position.z = 0;
	output.Position.w = 1;
	
	output.TexCoords.x = ((output.Position.x + 1) / 2) * -1;
	output.TexCoords.y = (output.Position.y - 1) / 2;
	output.Color = lerp(xStartColor, xEndColor, xTime/(xDuration/1.8));
	float4 deathColor = float4(xEndColor.r, xEndColor.g, xEndColor.b, 0);
	output.Color = lerp(output.Color, deathColor, xTime/xDuration);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 colorOut = input.Color;
	if(xSample)
	{
		float4 texCol = tex2D(TextureSampler, input.TexCoords);
		colorOut = lerp(colorOut, texCol, colorOut.a);
		colorOut.a /= 4;
	}
	
    return colorOut;
}

technique Technique1
{
    pass Pass0
    {
		CULLMODE = NONE;
        ALPHABLENDENABLE = TRUE;
        SRCBLEND = SRCALPHA;
        DESTBLEND = ONE;
        ZENABLE = FALSE;

        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
    pass Pass1
    {
		CULLMODE = NONE;
        //POINTSIZE = xSize;
        ALPHABLENDENABLE = TRUE;
        SRCBLEND = SRCALPHA;
        DESTBLEND = INVSRCALPHA;
        ZENABLE = FALSE;

        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
}
