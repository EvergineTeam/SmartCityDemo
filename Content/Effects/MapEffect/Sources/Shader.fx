[Begin_ResourceLayout]

	[Directives:ColorSpace GAMMA_COLORSPACE_OFF GAMMA_COLORSPACE]

	cbuffer PerDrawCall : register(b0)
	{
		float4x4 WorldViewProj	: packoffset(c0);	[WorldViewProjection]
		float4x4 World 			: packoffset(c4);	[World]
		float4x4 ViewProj		: packoffset(c8);	[ViewProjection]
	};

	cbuffer Parameters : register(b1)
	{
		float3 Center			: packoffset(c0);  	[Default(0,0,0)]
		float  Radius			: packoffset(c0.w); [Default(1)]
		float  StartBorder		: packoffset(c1.x); [Default(0.9)]
		float  MinLevel			: packoffset(c1.y); [Default(0)]
	};
	
	Texture2D BaseColorTexture : register(t0);
	
	SamplerState BaseColorSampler : register(s0);

[End_ResourceLayout]

[Begin_Pass:ShadowMap]
	[Profile 10_0]
	[Entrypoints VS = VertexFunction PS=PixelFunction]
	[DepthClipEnable False]
	
	struct VSInputPbr
	{
		float3 Position : POSITION;
		uint InstId : SV_InstanceID;
	};
	
	struct VSOutputPbr
	{
		float4 PositionProj : SV_POSITION;
		float3 PositionWS : POSITION;
	
		#if MULTIVIEW
		uint ViewId : SV_RenderTargetArrayIndex;
		#endif
	};
	
	VSOutputPbr VertexFunction(VSInputPbr input)
	{
		VSOutputPbr output = (VSOutputPbr)0;
	
	 	const float4 transformedPosWorld = mul(float4(input.Position, 1), World);
	
	 	output.PositionWS = transformedPosWorld;
	 	output.PositionProj = mul(transformedPosWorld, ViewProj);
		return output;
	}	

	void PixelFunction(VSOutputPbr input)
	{
		if(input.PositionWS.y < MinLevel)
		{
			discard;
		}
		
		float distanceToCenter = length(input.PositionWS.xz - Center.xz);		
		
		if(distanceToCenter > Radius)
		{
			discard;
		}
	}

[End_Pass]

[Begin_Pass:Default]
	[Profile 10_0]
	[Entrypoints VS=VS PS=PS]

	struct VS_IN
	{
		float4 Position : POSITION;		
		float2 TexCoord : TEXCOORD;
	};

	struct PS_IN
	{
		float4 pos : SV_POSITION;
		float4 posWS : POSITION;
		float2 Tex : TEXCOORD;
	};
	
	#if !GAMMA_COLORSPACE
	
	float4 GammaToLinear(const float4 color)
	{
		return float4(pow(color.rgb, 2.2), color.a);
	}
	#endif

	PS_IN VS(VS_IN input)
	{
		PS_IN output = (PS_IN)0;

		output.pos = mul(input.Position, WorldViewProj);
		output.posWS = mul(input.Position, World);
		output.Tex = input.TexCoord;

		return output;
	}

	float4 PS(PS_IN input) : SV_Target
	{
		if(input.posWS.y < MinLevel)
		{
			discard;
		}
		float distanceToCenter = length(input.posWS.xz - Center.xz);		
		
		if(distanceToCenter > Radius)
		{
			discard;
		}
		
		float4 color = BaseColorTexture.Sample(BaseColorSampler, input.Tex);		
		
#if !GAMMA_COLORSPACE
		color = GammaToLinear(color);
#endif		

		color.a = 1 - saturate((distanceToCenter - StartBorder)/(Radius - StartBorder));
		color.rgb *= color.a;
		
		return color;
	}

[End_Pass]