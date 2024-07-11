shader_type spatial;
render_mode unshaded,blend_add;
uniform sampler2D environment : filter_linear;
uniform sampler2D noise : filter_linear;
uniform sampler2D depth : hint_depth_texture;
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
	vec2 uv = UV2 + vec2(TIME *-1.0,0.0);
	vec2 matcap_uv = (generate_matcap_uv(NORMAL, NODE_POSITION_VIEW)); // Matcap UV's
	matcap_uv = mix(matcap_uv,SCREEN_UV,0.2);
	vec3 _matcap = texture(environment, matcap_uv).rgb; // Matcap Texture
	float met = texture(noise,UV).r;
	met = met * met;
	float noisepx = texture(noise,uv).x;
	float sine = sin(TIME * PI * 2.0);
	sine = 1.0 - 0.6 * sine * sine * sine* sine* sine * sine * sine* sine;
	float d = texture(depth, SCREEN_UV).x;
	d = PROJECTION_MATRIX[3][2] /(d + PROJECTION_MATRIX[2][2])*0.1;
	d = max(0,min(1,d * d));
	ALBEDO = d * noisepx * sine * pow(2.0 * _matcap ,vec3(2.5))* met; 
}

