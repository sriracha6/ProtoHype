Shader "Hidden/Water"
{
    Properties
    {
		_UITime ("Speed" , float) = 0.5
		_UIZoom ("Zoom" , float) = 7.0
		_Waves ("Waves & Alpha" , float) = (0.5,0.2,0.5,3.0)
        _MainTex ("Texture", 2D) = "white" {}    
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _UITime;
	        float _UIZoom;
	        float4 _Waves;	

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                		float4 uv_texture = tex2D(_MainTex, i.uv);
		
		        float4 k = _Time*_UITime;
		        k.xy = i.uv * _UIZoom;
		    
		        float val1 = length(0.5-frac(k.xyw=mul(float3x3(float3(-2.0,-1.0,0.0), float3(3.0,-1.0,1.0), float3(1.0,-1.0,-1.0)),k.xyw) * _Waves.x));
		        float val2 = length(0.5-frac(k.xyw=mul(float3x3(float3(-2.0,-1.0,0.0), float3(3.0,-1.0,1.0), float3(1.0,-1.0,-1.0)),k.xyw) * _Waves.y));
		        float val3 = length(0.5-frac(k.xyw=mul(float3x3(float3(-2.0,-1.0,0.0), float3(3.0,-1.0,1.0), float3(1.0,-1.0,-1.0)),k.xyw) * _Waves.z));
		
		        return ( pow(min(min(val1,val2),val3), 7.0) * _Waves.w)+uv_texture;
                        return col;
            }
            ENDCG
        }
    }
}
