//-----------------------------------------
//	Skinning
//-----------------------------------------


// REQUIRED BY CAL3D PROCESSOR, NOT USED

//------------------
//--- Parameters ---
#define MaxBones 60
float4x4 Bones[MaxBones];

float4x4 World : WORLD;
float4x4 View : VIEW;
float4x4 Projection : PROJECTION;

//light properties
float3 lightPosition;
float4 ambientLightColor;
float4 diffuseLightColor;

texture2D Texture;

sampler2D DiffuseTextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};


//--------------------
//--- VertexShader ---

struct VertexInput
{
	float3 position			: POSITION0;
	float2 texCoord			: TEXCOORD0;
	float3 normal			: NORMAL0;
	float4 BoneIndices		: BLENDINDICES0;
	float4 BoneWeights		: BLENDWEIGHT0;
};

struct VertexOutput
{
	float4 Position			: POSITION0;
	float3 WorldNormal		: COLOR0;
	float2 texCoord			: TEXCOORD0;
	float3 WorldPosition	: TEXCOORD1;	
};

VertexOutput VertexShader( VertexInput input )
{
	VertexOutput output = (VertexOutput)0;

	// Blend between the weighted bone matrices.
	float4x4 skinTransform = 0;
  
	skinTransform += Bones[input.BoneIndices.x] * input.BoneWeights.x;
	skinTransform += Bones[input.BoneIndices.y] * input.BoneWeights.y;
	skinTransform += Bones[input.BoneIndices.z] * input.BoneWeights.z;
	skinTransform += Bones[input.BoneIndices.w] * input.BoneWeights.w;
	
	// Skin the vertex position.
	float4 position = mul(float4(input.position,1.0f), skinTransform);
	//float4 position = float4(input.position,1.0f);
    
	float4x4 wvp = mul(mul(World, View), Projection);
	output.Position = mul(position, wvp);
	output.texCoord = input.texCoord;
	
	output.WorldNormal =  normalize(mul(input.normal, World));
	float4 worldPosition =  mul(float4(input.position, 1.0), World);
	output.WorldPosition = worldPosition / worldPosition.w;
	
	return ( output );
}

//-------------------
//--- PixelShader ---
float4 PixelShader( VertexOutput input ) : COLOR
{
	//calculate per-pixel diffuse
	float3 directionToLight = normalize(lightPosition - input.WorldPosition);
	float diffuseIntensity = saturate( dot(directionToLight, input.WorldNormal));
	float4 diffuse = diffuseLightColor * diffuseIntensity;
	float4 color = tex2D(DiffuseTextureSampler, input.texCoord);

	color = color * (diffuse + ambientLightColor);
	color.a = 1.0;

	return color;
}

//------------------
//--- Techniques ---

Technique Skinning
{
    Pass Go
    {
		VertexShader = compile vs_2_0 VertexShader();
		PixelShader = compile ps_2_0 PixelShader();
    }
}
