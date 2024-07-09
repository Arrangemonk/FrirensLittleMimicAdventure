shader_type spatial;
render_mode unshaded,blend_add;
uniform sampler2D environment : filter_linear;
uniform sampler2D diffuse : filter_linear;
uniform sampler2D metallic : filter_linear;
uniform sampler2D noise : filter_linear;
void vertex() {
	// Called for every vertex the material is visible on.
}

float random (vec2 uv) {
    return fract(sin(dot(uv.xy,
        vec2(12.9898,78.233))) * 43758.5453123);
}

vec2 generate_matcap_uv(vec3 _normal, vec3 _node_position_view) {
	vec3 cross_space = cross(normalize(_node_position_view), _normal); // cross products the normal with it's base position
	vec2 mat_space = vec2(cross_space.y, cross_space.x); // converts to a Vec2 UV, having to switch x and y
	mat_space *= vec2(-0.5, -0.5); // modifies the mat space's size to fit into uv coords, & flips it
	mat_space += vec2(0.5, 0.5); // transforms the mat space into uv coords
	return mat_space; // export the final uv's
	//return _node_position_view.xz;
}

void fragment(){
	vec2 uv = UV2 + vec2(TIME *-1.0,TIME *0.2);
	vec2 rnd = vec2(random(UV),random(UV* 2.0));
	vec2 matcap_uv = (generate_matcap_uv(NORMAL, NODE_POSITION_VIEW)); // Matcap UV's
	matcap_uv = mix(matcap_uv,SCREEN_UV,0.2) + rnd * 0.001;
	vec3 _matcap = texture(environment, matcap_uv).rgb; // Matcap Texture
	float met = texture(metallic,UV + rnd * 0.0006).r;
	vec3 diff = texture(diffuse,UV + rnd * 0.0006).xyz;
	float noisepx = texture(noise,uv + rnd * 0.0006).x;
	float sine = 1.0 - abs(pow(sin(TIME *PI * 2.0),10))*0.85;
	ALBEDO = noisepx * sine * pow(2.0 *diff * _matcap * met + diff * (1.01 - met),vec3(2.5));
	//ALBEDO = vec3(matcap_uv,1.0);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
