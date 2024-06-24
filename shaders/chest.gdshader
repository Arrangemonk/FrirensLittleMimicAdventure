shader_type spatial;
render_mode unshaded;

uniform sampler2D albedo : filter_nearest;
uniform sampler2D lightmap : filter_linear_mipmap_anisotropic;


void fragment() {
	vec4 world_pos = inverse(VIEW_MATRIX)  * vec4(VERTEX,1.0);
	//world_pos = world_pos / world_pos.w;
	vec3 alb = texture(albedo, UV).xyz;
    vec3 light = texture(lightmap, vec2(world_pos.x * 0.1 + 0.5,0.75 - world_pos.z * 0.05)).xyz;
	ALBEDO = pow(2.0 * alb * light * ((NORMAL.y * 0.4) + 0.6),vec3(2.5));
	//ALBEDO = world_pos.xyz;
}