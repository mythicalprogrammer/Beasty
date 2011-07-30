float4x4 World;
float4x4 View;
float4x4 Projection;

float EdgeWidth = 0.8f;
float EdgeIntensity = 0.6;

// How sensitive should the edge detection be to tiny variations in the input data?
// Smaller settings will make it pick up more subtle edges, while larger values get
// rid of unwanted noise.
float NormalThreshold = 0.5;
float DepthThreshold = 0.1;

// How dark should the edges get in response to changes in the input data?
float NormalSensitivity = 1;
float DepthSensitivity = 10;

// Pass in the current screen resolution.
float2 ScreenResolution;

float TexOffset;

texture PencilTex;

sampler PencilSampler = sampler_state
{
    Texture = <PencilTex>;

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture SceneTexture;

sampler SceneSampler : register(s0) = sampler_state
{
    Texture = (SceneTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


// This texture contains normals (in r and g color channels) and depth (in alpha)
// for the main scene image. Differences in the normal and depth data are used
// to detect where the edges of the model are.
texture NormalDepthTexture;

sampler NormalDepthSampler : register(s1) = sampler_state
{
    Texture = (NormalDepthTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};

//----------------------------
// Pixel Shader
//----------------------------

float4 PixelShader(float2 texCoord : TEXCOORD0, uniform bool computeEdge, 
	uniform bool showSketch) : COLOR0
{
    // Look up the original color from the main scene.
    float3 scene = tex2D(SceneSampler, texCoord);
    
    if (computeEdge)
    {
		// Apply the edge detection filter?
		// Look up four values from the normal/depth texture, offset along the
		// four diagonals from the pixel we are currently shading.
		float2 edgeOffset = EdgeWidth / ScreenResolution;
	    
	    
		float4 n1 = tex2D(NormalDepthSampler, texCoord + float2(-1, -1) * edgeOffset);
		float4 n2 = tex2D(NormalDepthSampler, texCoord + float2( 1,  1) * edgeOffset);
		float4 n3 = tex2D(NormalDepthSampler, texCoord + float2(-1,  1) * edgeOffset);
		float4 n4 = tex2D(NormalDepthSampler, texCoord + float2( 1, -1) * edgeOffset);
	    
		n1.z = sqrt(1.0f-pow(n1.x,2)-pow(n1.y,2));
		n2.z = sqrt(1.0f-pow(n2.x,2)-pow(n2.y,2));
		n3.z = sqrt(1.0f-pow(n3.x,2)-pow(n3.y,2));
		n4.z = sqrt(1.0f-pow(n4.x,2)-pow(n4.y,2));

		// Work out how much the normal and depth values are changing.
		float4 diagonalDelta = abs(n1 - n2) + abs(n3 - n4);

		float normalDelta = dot(diagonalDelta.xyz, 1);
		float depthDelta = diagonalDelta.w;
	    
		// Filter out very small changes, in order to produce nice clean results.
		normalDelta = saturate((normalDelta - NormalThreshold) * NormalSensitivity);
		depthDelta = saturate((depthDelta - DepthThreshold) * DepthSensitivity);

		// Does this pixel lie on an edge?
		float edgeAmount = saturate(normalDelta + depthDelta) * EdgeIntensity;
	    
		// Apply the edge detection result to the main scene color.
		scene *= (1 - edgeAmount);
    }
    
    float4 pincelColor = 0;
    if(showSketch)
    {
		float intensity = tex2D(NormalDepthSampler, texCoord).b;
		pincelColor = (1.0-intensity) * (1.0-tex2D(PencilSampler, (texCoord + TexOffset) * 8).r);
    }
	
    return float4(scene - pincelColor.rgb, 1) ;
}

//----------------------------
// Technique
//----------------------------

technique EdgeDetect
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShader(true, true);
    }
}

technique NoEdgeDetect
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShader(false, true);
    }
}

technique NoCel
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShader(false, false);
    }
}