# IvanMurzak/Unity-MCP Feature Supersession Tracker

> [!NOTE]
> **IvanMurzak/Unity-MCP Version:** `mcpforunityserver` **10.3.0** (synced via temp release cache).
> **Baseline:** Unity Pipeline package `com.unity.pipeline` (177 native CLI commands + compatibility extensions).
> **Updated via:** `sync_ivanmurzak_tracker.py` (uses `$TEMP/unity-mcp-sync` for release checks)

| IvanMurzak/Unity-MCP feature | superseeded? | why not |
| :--- | :---: | :--- |
| apply_text_edits | yes |  |
| batch_execute | yes |  |
| create_script | yes |  |
| debug_request_context | no | IvanMurzak MCP-transport specific diagnostic for inspectable JSON-RPC headers and transport debugging |
| delete_script | yes |  |
| execute_code: execute | yes |  |
| execute_code: get_history | no | Pipeline script eval does not maintain or expose execution history ledger |
| execute_code: replay | no | Pipeline lacks snippet replay capability from execution history |
| execute_code: clear_history | no | Pipeline lacks snippet execution history buffer management |
| execute_custom_tool | yes |  |
| execute_menu_item | yes |  |
| find_gameobjects | yes |  |
| find_in_file | yes |  |
| generate_audio: generate | no | Pipeline does not integrate generative AI audio synthesis providers (e.g. ElevenLabs) |
| generate_audio: status | no | Pipeline lacks async generative audio job polling |
| generate_audio: cancel | no | Pipeline lacks async generative audio job cancellation |
| generate_audio: list_providers | no | Pipeline lacks generative audio provider discovery |
| generate_image: generate | no | Pipeline does not integrate generative AI image generation providers |
| generate_image: remove_background | no | Pipeline lacks AI background removal processing on textures |
| generate_image: status | no | Pipeline lacks async generative image job polling |
| generate_image: cancel | no | Pipeline lacks async generative image job cancellation |
| generate_image: list_providers | no | Pipeline lacks generative image provider discovery |
| generate_model: generate | no | Pipeline does not integrate generative 3D mesh synthesis providers (Tripo3D/Meshy) |
| generate_model: status | no | Pipeline lacks async generative 3D model job polling |
| generate_model: cancel | no | Pipeline lacks async generative 3D model job cancellation |
| generate_model: list_providers | no | Pipeline lacks generative 3D model provider discovery |
| get_sha | yes |  |
| get_test_job | yes |  |
| import_model: search | no | Pipeline lacks external 3D asset marketplace/model repository search (Sketchfab) |
| import_model: preview | no | Pipeline lacks remote 3D asset preview thumbnail extraction |
| import_model: import | no | Pipeline lacks automated external 3D asset downloading and rig setup |
| import_model: status | no | Pipeline lacks async 3D model download job polling |
| import_model: cancel | no | Pipeline lacks async 3D model download job cancellation |
| import_model: list_providers | no | Pipeline lacks 3D asset repository provider listing |
| import_model_file | no | Pipeline asset import does not feature native gltf import, automated 3D mesh bounds normalization, or target size scaling |
| manage_animation: animator_get_info | yes |  |
| manage_animation: animator_get_parameter | yes |  |
| manage_animation: animator_play | no | Pipeline only provides authoring-time controller modification; lacks runtime Animator playback control (play, crossfade, set speed) |
| manage_animation: animator_crossfade | no | Pipeline lacks runtime Animator crossfade command |
| manage_animation: animator_set_parameter | no | Pipeline animation.animator only configures controller parameter definitions; lacks runtime parameter manipulation on live Animator |
| manage_animation: animator_set_speed | no | Pipeline lacks runtime Animator playback speed control |
| manage_animation: animator_set_enabled | no | Pipeline lacks dedicated Animator component enable/disable toggle |
| manage_animation: controller_create | yes |  |
| manage_animation: controller_add_state | yes |  |
| manage_animation: controller_add_transition | yes |  |
| manage_animation: controller_add_parameter | yes |  |
| manage_animation: controller_get_info | yes |  |
| manage_animation: controller_assign | yes |  |
| manage_animation: controller_add_layer | yes |  |
| manage_animation: controller_remove_layer | no | Pipeline animation.animator supports adding layers, but lacks controller layer removal |
| manage_animation: controller_set_layer_weight | no | Pipeline lacks controller layer weight and blending mode modification |
| manage_animation: controller_create_blend_tree_1d | no | Pipeline animation.animator does not support BlendTree creation or configuration |
| manage_animation: controller_create_blend_tree_2d | no | Pipeline animation.animator does not support 2D BlendTrees (Simple/Freeform) |
| manage_animation: controller_add_blend_tree_child | no | Pipeline animation.animator does not support adding motions or child trees to BlendTrees |
| manage_animation: clip_create | yes |  |
| manage_animation: clip_get_info | yes |  |
| manage_animation: clip_add_curve | yes |  |
| manage_animation: clip_set_curve | yes |  |
| manage_animation: clip_set_vector_curve | yes |  |
| manage_animation: clip_create_preset | no | Pipeline lacks pre-baked procedural animation presets (bounce, pulse, shake, rotate) |
| manage_animation: clip_assign | yes |  |
| manage_animation: clip_add_event | no | Pipeline animation.clip lacks AnimationEvent manipulation (add/remove event keyframes) |
| manage_animation: clip_remove_event | no | Pipeline animation.clip lacks AnimationEvent removal |
| manage_asset: import | yes |  |
| manage_asset: create | yes |  |
| manage_asset: modify | yes |  |
| manage_asset: delete | yes |  |
| manage_asset: duplicate | yes |  |
| manage_asset: move | yes |  |
| manage_asset: rename | yes |  |
| manage_asset: search | yes |  |
| manage_asset: get_info | yes |  |
| manage_asset: create_folder | yes |  |
| manage_asset: get_components | yes |  |
| manage_build: build | yes |  |
| manage_build: status | yes |  |
| manage_build: platform | yes |  |
| manage_build: settings | yes |  |
| manage_build: scenes | yes |  |
| manage_build: profiles | yes |  |
| manage_build: batch | yes |  |
| manage_build: cancel | yes |  |
| manage_camera: ping | no | Pipeline does not detect or report Cinemachine package installation status or version |
| manage_camera: ensure_brain | no | Pipeline has no CinemachineBrain setup or configuration command |
| manage_camera: get_brain_status | no | Pipeline has no CinemachineBrain blend and active virtual camera monitoring |
| manage_camera: create_camera | no | Pipeline lacks specialized camera creation with presets (third_person, freelook, follow, dolly, top_down) and Cinemachine rig instantiation |
| manage_camera: set_target | no | Pipeline lacks high-level Follow/LookAt camera target assignment |
| manage_camera: set_priority | no | Pipeline lacks camera priority management for Cinemachine brain selection |
| manage_camera: set_lens | yes |  |
| manage_camera: set_body | no | Pipeline does not support Cinemachine Body component configuration (Transposer, FramingTransposer, OrbitalTransposer) |
| manage_camera: set_aim | no | Pipeline does not support Cinemachine Aim component configuration (Composer, GroupComposer) |
| manage_camera: set_noise | no | Pipeline does not support Cinemachine Noise (BasicMultiChannelPerlin) amplitude/frequency configuration |
| manage_camera: add_extension | no | Pipeline does not support Cinemachine extensions (Confiner2D, Deoccluder, ImpulseListener, FollowZoom) |
| manage_camera: remove_extension | no | Pipeline lacks Cinemachine extension removal |
| manage_camera: set_blend | no | Pipeline does not support Cinemachine blend style (Cut, EaseInOut, Linear) and duration configuration |
| manage_camera: force_camera | no | Pipeline does not support forcing Cinemachine brain to specific virtual camera |
| manage_camera: release_override | no | Pipeline lacks Cinemachine camera override release |
| manage_camera: list_cameras | no | Pipeline lacks scene-wide camera discovery reporting Camera + CinemachineCamera priorities and targets |
| manage_camera: screenshot | yes |  |
| manage_camera: screenshot_multiview | no | Pipeline screenshot captures single view; lacks 6-angle surround contact sheet generation |
| manage_components: add | yes |  |
| manage_components: remove | yes |  |
| manage_components: set_property | yes |  |
| manage_editor: telemetry_status | no | IvanMurzak MCP telemetry ping and connection diagnostic |
| manage_editor: telemetry_ping | no | IvanMurzak MCP telemetry ping |
| manage_editor: play | yes |  |
| manage_editor: pause | yes |  |
| manage_editor: stop | yes |  |
| manage_editor: set_active_tool | no | Pipeline does not support setting active Unity Editor tool (Move, Rotate, Scale, Rect, Transform) |
| manage_editor: add_tag | yes |  |
| manage_editor: remove_tag | yes |  |
| manage_editor: add_layer | yes |  |
| manage_editor: remove_layer | yes |  |
| manage_editor: deploy_package | no | IvanMurzak tool to hot-deploy MCP package source into project Packages/ folder |
| manage_editor: restore_package | no | IvanMurzak tool to revert hot-deployed package source from backup |
| manage_editor: undo | no | Pipeline lacks remote Undo command (Undo.PerformUndo) |
| manage_editor: redo | no | Pipeline lacks remote Redo command (Undo.PerformRedo) |
| manage_gameobject: create | yes |  |
| manage_gameobject: modify | yes |  |
| manage_gameobject: delete | yes |  |
| manage_gameobject: duplicate | yes |  |
| manage_gameobject: move_relative | no | Pipeline gameobject set_transform sets absolute position/rotation; lacks relative translation vector application |
| manage_gameobject: look_at | no | Pipeline lacks transform.LookAt orienting command |
| manage_graphics: ping | no | Pipeline does not report active render pipeline type (Built-in, URP, HDRP) and supported feature matrix |
| manage_graphics: volume_create | no | Pipeline does not support URP/HDRP Volume component creation |
| manage_graphics: volume_add_effect | no | Pipeline does not support adding VolumeComponent post-processing effects (Bloom, Vignette, ColorAdjustments, etc.) |
| manage_graphics: volume_set_effect | no | Pipeline does not support modifying VolumeComponent post-processing effect parameters |
| manage_graphics: volume_remove_effect | no | Pipeline does not support removing VolumeComponent post-processing effects |
| manage_graphics: volume_get_info | no | Pipeline lacks Volume inspection listing profile effects and override states |
| manage_graphics: volume_set_properties | no | Pipeline lacks Volume component property modification (weight, priority, isGlobal) |
| manage_graphics: volume_list_effects | no | Pipeline lacks listing available post-processing effect types supported by current SRP |
| manage_graphics: volume_create_profile | no | Pipeline lacks VolumeProfile asset creation |
| manage_graphics: bake_start | yes |  |
| manage_graphics: bake_cancel | yes |  |
| manage_graphics: bake_status | yes |  |
| manage_graphics: bake_clear | yes |  |
| manage_graphics: bake_reflection_probe | no | Pipeline lighting bake runs scene lightmapping; does not support individual reflection probe baking |
| manage_graphics: bake_get_settings | yes |  |
| manage_graphics: bake_set_settings | yes |  |
| manage_graphics: bake_create_light_probe_group | no | Pipeline lacks LightProbeGroup creation and automated probe placement |
| manage_graphics: bake_create_reflection_probe | no | Pipeline lacks ReflectionProbe creation and configuration command |
| manage_graphics: bake_set_probe_positions | no | Pipeline lacks LightProbe probe position vector array assignment |
| manage_graphics: stats_get | no | Pipeline diagnostics get_performance returns process CPU/memory; does not return Unity rendering statistics (draw calls, set-pass calls, triangles, vertices, batches) |
| manage_graphics: stats_list_counters | no | Pipeline lacks ProfilerRecorder/rendering counter discovery |
| manage_graphics: stats_set_scene_debug | no | Pipeline lacks SceneView debug draw mode toggling (Wireframe, Overdraw, Mipmaps) |
| manage_graphics: stats_get_memory | yes |  |
| manage_graphics: pipeline_get_info | yes |  |
| manage_graphics: pipeline_set_quality | yes |  |
| manage_graphics: pipeline_get_settings | yes |  |
| manage_graphics: pipeline_set_settings | yes |  |
| manage_graphics: feature_list | no | Pipeline lacks URP ScriptableRendererFeature inspection on ForwardRendererData |
| manage_graphics: feature_add | no | Pipeline lacks adding ScriptableRendererFeatures (RenderObjects, Decal, FullScreenPass) to URP renderer |
| manage_graphics: feature_remove | no | Pipeline lacks removing ScriptableRendererFeatures from URP renderer |
| manage_graphics: feature_configure | no | Pipeline lacks configuring ScriptableRendererFeature parameters |
| manage_graphics: feature_toggle | no | Pipeline lacks toggling ScriptableRendererFeature active state |
| manage_graphics: feature_reorder | no | Pipeline lacks reordering ScriptableRendererFeature execution index |
| manage_graphics: skybox_get | no | Pipeline lacks RenderSettings environment lighting, skybox material, and fog read query |
| manage_graphics: skybox_set_material | no | Pipeline lacks setting RenderSettings.skybox material |
| manage_graphics: skybox_set_properties | no | Pipeline lacks modifying active skybox material shader properties |
| manage_graphics: skybox_set_ambient | no | Pipeline lacks configuring RenderSettings ambient lighting mode, colors, and intensity |
| manage_graphics: skybox_set_fog | no | Pipeline lacks configuring RenderSettings fog (mode, density, color, distances) |
| manage_graphics: skybox_set_reflection | no | Pipeline lacks configuring RenderSettings default reflection mode and resolution |
| manage_graphics: skybox_set_sun | no | Pipeline lacks assigning RenderSettings.sun directional light source |
| manage_material: ping | no | Utility health check for material sub-system |
| manage_material: create | yes |  |
| manage_material: set_material_shader_property | no | Pipeline has 'material get_shader_properties' to inspect shader properties, but lacks dedicated command to set shader property values directly on material assets |
| manage_material: set_material_color | no | Pipeline lacks high-level command to set material color properties by name (_Color, _BaseColor) |
| manage_material: assign_material_to_renderer | no | Pipeline lacks direct convenience command to assign a material asset to a scene GameObject's Renderer |
| manage_material: set_renderer_color | no | Pipeline lacks direct command to set MaterialPropertyBlock or renderer color without asset mutation |
| manage_material: get_material_info | yes |  |
| manage_packages: list_packages | yes |  |
| manage_packages: search_packages | yes |  |
| manage_packages: get_package_info | yes |  |
| manage_packages: ping | no | Package manager availability ping |
| manage_packages: status | yes |  |
| manage_packages: add_package | yes |  |
| manage_packages: remove_package | yes |  |
| manage_packages: list_registries | no | Pipeline lacks manifest scopedRegistries query |
| manage_packages: add_registry | no | Pipeline lacks adding scoped registries (e.g. OpenUPM) to packages-manifest.json |
| manage_packages: remove_registry | no | Pipeline lacks removing scoped registries from packages-manifest.json |
| manage_packages: embed_package | no | Pipeline lacks embedding installed registry package into local Packages folder for editing |
| manage_packages: resolve_packages | yes |  |
| manage_physics: ping | no | Physics sub-system availability ping |
| manage_physics: get_settings | yes |  |
| manage_physics: set_settings | yes |  |
| manage_physics: get_collision_matrix | no | Pipeline lacks reading 32x32 Physics collision layer matrix |
| manage_physics: set_collision_matrix | no | Pipeline lacks setting layer collision masks (Physics.IgnoreLayerCollision) |
| manage_physics: create_physics_material | no | Pipeline lacks creating PhysicsMaterial / PhysicMaterial assets |
| manage_physics: configure_physics_material | no | Pipeline lacks configuring PhysicMaterial friction and bounciness properties |
| manage_physics: assign_physics_material | no | Pipeline lacks assigning PhysicMaterial to Collider components |
| manage_physics: add_joint | no | Pipeline lacks adding and configuring 2D/3D Joint components (Hinge, Fixed, Spring, Character, Configurable) |
| manage_physics: configure_joint | no | Pipeline lacks configuring Joint limits, motor, spring, and connected bodies |
| manage_physics: remove_joint | yes |  |
| manage_physics: raycast | no | Pipeline lacks simulated Physics.Raycast query execution |
| manage_physics: raycast_all | no | Pipeline lacks simulated Physics.RaycastAll query execution |
| manage_physics: linecast | no | Pipeline lacks simulated Physics.Linecast query execution |
| manage_physics: shapecast | no | Pipeline lacks simulated Physics.BoxCast / SphereCast / CapsuleCast shape casts |
| manage_physics: overlap | no | Pipeline lacks simulated Physics.OverlapSphere / OverlapBox spatial query |
| manage_physics: validate | no | Pipeline lacks physics scene validation (detecting missing colliders, non-convex mesh colliders with rigidbodies) |
| manage_physics: simulate_step | no | Pipeline lacks stepping physics simulation explicitly (Physics.Simulate) |
| manage_physics: apply_force | no | Pipeline lacks applying physics force or torque to Rigidbody (Rigidbody.AddForce / AddExplosionForce) |
| manage_physics: get_rigidbody | yes |  |
| manage_physics: configure_rigidbody | yes |  |
| manage_prefabs: create_from_gameobject | yes |  |
| manage_prefabs: get_info | yes |  |
| manage_prefabs: get_hierarchy | yes |  |
| manage_prefabs: modify_contents | yes |  |
| manage_prefabs: open_prefab_stage | no | Pipeline lacks opening PrefabStage in isolation mode for focused editing |
| manage_prefabs: save_prefab_stage | no | Pipeline lacks saving open PrefabStage |
| manage_prefabs: close_prefab_stage | no | Pipeline lacks closing PrefabStage and returning to main scene |
| manage_probuilder: ping | no | Pipeline does not detect ProBuilder package installation or capability |
| manage_probuilder: create_shape | no | Pipeline lacks ProBuilder 3D primitive shape generation (Cube, Prism, Cylinder, Arch, etc.) |
| manage_probuilder: create_poly_shape | no | Pipeline lacks ProBuilder 2D polygon footprint extrusion |
| manage_probuilder: extrude_faces | no | Pipeline lacks ProBuilder face extrusion modeling operations |
| manage_probuilder: extrude_edges | no | Pipeline lacks ProBuilder edge extrusion operations |
| manage_probuilder: bevel_edges | no | Pipeline lacks ProBuilder edge beveling operations |
| manage_probuilder: subdivide | no | Pipeline lacks ProBuilder mesh face subdivision |
| manage_probuilder: delete_faces | no | Pipeline lacks ProBuilder face deletion |
| manage_probuilder: bridge_edges | no | Pipeline lacks ProBuilder edge bridging between open boundaries |
| manage_probuilder: connect_elements | no | Pipeline lacks ProBuilder element connecting (inserting edges between vertices) |
| manage_probuilder: detach_faces | no | Pipeline lacks detaching ProBuilder faces to sub-mesh or new object |
| manage_probuilder: flip_normals | no | Pipeline lacks flipping ProBuilder face normals |
| manage_probuilder: merge_faces | no | Pipeline lacks merging coplanar ProBuilder faces |
| manage_probuilder: combine_meshes | no | Pipeline lacks combining multiple ProBuilder meshes |
| manage_probuilder: merge_objects | no | Pipeline lacks merging multiple GameObjects into single ProBuilder object |
| manage_probuilder: duplicate_and_flip | no | Pipeline lacks creating double-sided ProBuilder geometry |
| manage_probuilder: create_polygon | no | Pipeline lacks constructing new ProBuilder polygon from vertices |
| manage_probuilder: merge_vertices | no | Pipeline lacks collapsing ProBuilder vertices to single point |
| manage_probuilder: weld_vertices | no | Pipeline lacks welding proximity ProBuilder vertices within radius |
| manage_probuilder: split_vertices | no | Pipeline lacks splitting shared ProBuilder vertices |
| manage_probuilder: move_vertices | no | Pipeline lacks translating ProBuilder vertex positions |
| manage_probuilder: insert_vertex | no | Pipeline lacks inserting vertex on ProBuilder edge or face |
| manage_probuilder: append_vertices_to_edge | no | Pipeline lacks inserting evenly-spaced vertices on ProBuilder edges |
| manage_probuilder: select_faces | no | Pipeline lacks semantic face selection by direction (top, bottom, normal) and angle |
| manage_probuilder: set_face_material | no | Pipeline lacks assigning material to specific ProBuilder face indices |
| manage_probuilder: set_face_color | no | Pipeline lacks painting vertex colors on ProBuilder faces |
| manage_probuilder: set_face_uvs | no | Pipeline lacks configuring ProBuilder UV auto-unwrap and projection parameters |
| manage_probuilder: get_mesh_info | no | Pipeline lacks ProBuilder mesh inspection (face normals, bounds, vertex counts) |
| manage_probuilder: convert_to_probuilder | no | Pipeline lacks converting standard Unity MeshFilter meshes into ProBuilder editable mesh |
| manage_probuilder: set_smoothing | no | Pipeline lacks setting ProBuilder smoothing groups |
| manage_probuilder: auto_smooth | no | Pipeline lacks auto-assigning smoothing groups by angle threshold |
| manage_probuilder: center_pivot | no | Pipeline lacks centering ProBuilder pivot to mesh bounds |
| manage_probuilder: set_pivot | no | Pipeline lacks setting ProBuilder pivot to arbitrary world coordinates |
| manage_probuilder: freeze_transform | no | Pipeline lacks baking GameObject transform into ProBuilder vertex data |
| manage_probuilder: validate_mesh | no | Pipeline lacks ProBuilder mesh health validation (degenerate triangles, unused vertices) |
| manage_probuilder: repair_mesh | no | Pipeline lacks auto-repairing ProBuilder degenerate triangles and unused vertices |
| manage_profiler: profiler_start | yes |  |
| manage_profiler: profiler_stop | yes |  |
| manage_profiler: profiler_status | yes |  |
| manage_profiler: profiler_set_areas | no | Pipeline lacks toggling specific ProfilerArea channels (CPU, GPU, Memory, Audio, Physics) |
| manage_profiler: get_frame_timing | no | Pipeline lacks FrameTimingManager synchronous frame metrics (CPU/GPU frame time, jitter) |
| manage_profiler: get_counters | no | Pipeline lacks querying specific named ProfilerRecorder performance counters by category |
| manage_profiler: get_object_memory | no | Pipeline lacks querying runtime memory footprint for specific object path via Profiler.GetRuntimeMemorySizeLong |
| manage_profiler: memory_take_snapshot | yes |  |
| manage_profiler: memory_list_snapshots | no | Pipeline lacks listing existing .snap snapshot files on disk |
| manage_profiler: memory_compare_snapshots | no | Pipeline lacks automated memory snapshot delta diffing |
| manage_profiler: frame_debugger_enable | no | Pipeline lacks Frame Debugger session activation |
| manage_profiler: frame_debugger_disable | no | Pipeline lacks Frame Debugger deactivation |
| manage_profiler: frame_debugger_get_events | no | Pipeline lacks Frame Debugger draw call event list extraction |
| manage_profiler: ping | no | Profiler subsystem ping |
| manage_scene: create | yes |  |
| manage_scene: load | yes |  |
| manage_scene: save | yes |  |
| manage_scene: get_hierarchy | yes |  |
| manage_scene: get_active | yes |  |
| manage_scene: get_build_settings | yes |  |
| manage_scene: scene_view_frame | yes |  |
| manage_scene: close_scene | no | Pipeline lacks closing individual additive scene without saving all |
| manage_scene: set_active_scene | yes |  |
| manage_scene: get_loaded_scenes | yes |  |
| manage_scene: move_to_scene | no | Pipeline lacks moving GameObject between open scenes (SceneManager.MoveGameObjectToScene) |
| manage_scene: validate | no | Pipeline lacks scene health validation (detecting broken missing scripts, unassigned references) |
| manage_script: create | yes |  |
| manage_script: read | yes |  |
| manage_script: delete | yes |  |
| manage_script_capabilities | no | IvanMurzak MCP protocol negotiation tool reporting supported script compilers and Roslyn capability |
| manage_scriptable_object: create | yes |  |
| manage_scriptable_object: modify | no | Pipeline asset write_text only writes raw text; lacks structured property patching on ScriptableObject instances via SerializedObject |
| manage_shader: create | yes |  |
| manage_shader: read | yes |  |
| manage_shader: update | yes |  |
| manage_shader: delete | yes |  |
| manage_texture: create | no | Pipeline lacks procedural Texture2D generation (width, height, fill color) and PNG encoding |
| manage_texture: modify | no | Pipeline lacks procedural pixel modification on Texture2D assets |
| manage_texture: delete | yes |  |
| manage_texture: create_sprite | no | Pipeline lacks creating Sprite sub-assets from Texture2D with custom pivot/borders |
| manage_texture: apply_pattern | no | Pipeline lacks procedural texture pattern generation (checkerboard, stripes, grid, dots) |
| manage_texture: apply_gradient | no | Pipeline lacks procedural gradient generation (linear, radial, angle, color stops) |
| manage_texture: apply_noise | no | Pipeline lacks procedural Perlin/Simplex noise generation on Texture2D |
| manage_texture: set_import_settings | yes |  |
| manage_tools: list_groups | no | IvanMurzak MCP dynamic tool-group activation and management |
| manage_tools: activate | no | IvanMurzak MCP tool group activation |
| manage_tools: deactivate | no | IvanMurzak MCP tool group deactivation |
| manage_tools: sync | no | IvanMurzak MCP tool sync with connected editor |
| manage_tools: reset | no | IvanMurzak MCP tool configuration reset |
| manage_ui: ping | no | UI Toolkit subsystem ping |
| manage_ui: create | yes |  |
| manage_ui: read | yes |  |
| manage_ui: update | yes |  |
| manage_ui: delete | yes |  |
| manage_ui: attach_ui_document | no | Pipeline lacks attaching UIDocument component with PanelSettings and VisualTreeAsset link |
| manage_ui: detach_ui_document | yes |  |
| manage_ui: create_panel_settings | no | Pipeline lacks creating PanelSettings assets for UI Toolkit rendering |
| manage_ui: update_panel_settings | no | Pipeline lacks configuring PanelSettings (scale mode, reference resolution, sort order) |
| manage_ui: get_visual_tree | no | Pipeline lacks runtime VisualElement visual tree hierarchy inspection |
| manage_ui: render_ui | yes |  |
| manage_ui: link_stylesheet | no | Pipeline lacks linking USS style sheet to UXML visual tree |
| manage_ui: list | yes |  |
| manage_ui: modify_visual_element | no | Pipeline lacks live VisualElement style/class/text modification at runtime |
| manage_vfx: particle_create | no | Pipeline lacks ParticleSystem component creation and initial configuration |
| manage_vfx: particle_get_info | no | Pipeline lacks detailed ParticleSystem module and emitter inspection |
| manage_vfx: particle_set_main | no | Pipeline lacks ParticleSystem MainModule configuration (startSpeed, startSize, duration, loop) |
| manage_vfx: particle_set_emission | no | Pipeline lacks ParticleSystem EmissionModule configuration |
| manage_vfx: particle_set_shape | no | Pipeline lacks ParticleSystem ShapeModule configuration |
| manage_vfx: particle_set_color_over_lifetime | no | Pipeline lacks ParticleSystem ColorOverLifetimeModule gradient configuration |
| manage_vfx: particle_set_size_over_lifetime | no | Pipeline lacks ParticleSystem SizeOverLifetimeModule curve configuration |
| manage_vfx: particle_set_velocity_over_lifetime | no | Pipeline lacks ParticleSystem VelocityOverLifetimeModule configuration |
| manage_vfx: particle_set_noise | no | Pipeline lacks ParticleSystem NoiseModule configuration |
| manage_vfx: particle_set_renderer | no | Pipeline lacks ParticleSystemRenderer configuration |
| manage_vfx: particle_enable_module | no | Pipeline lacks ParticleSystem module toggle control |
| manage_vfx: particle_play | no | Pipeline lacks ParticleSystem playback trigger (play/pause/stop/restart) |
| manage_vfx: particle_stop | no | Pipeline lacks ParticleSystem playback stop |
| manage_vfx: particle_pause | no | Pipeline lacks ParticleSystem playback pause |
| manage_vfx: particle_restart | no | Pipeline lacks ParticleSystem playback restart |
| manage_vfx: particle_clear | no | Pipeline lacks clearing simulated particles from scene |
| manage_vfx: particle_add_burst | no | Pipeline lacks adding ParticleSystem emission bursts |
| manage_vfx: particle_clear_bursts | no | Pipeline lacks clearing emission bursts |
| manage_vfx: vfx_create_asset | no | Pipeline lacks VisualEffect Graph asset creation |
| manage_vfx: vfx_assign_asset | no | Pipeline lacks VisualEffect component asset binding |
| manage_vfx: vfx_list_templates | no | Pipeline lacks listing VFX Graph templates |
| manage_vfx: vfx_list_assets | no | Pipeline lacks listing VFX Graph assets in project |
| manage_vfx: vfx_get_info | no | Pipeline lacks inspecting VisualEffect component parameters and events |
| manage_vfx: vfx_set_float | no | Pipeline lacks setting VFX Graph exposed float parameter |
| manage_vfx: vfx_set_int | no | Pipeline lacks setting VFX Graph exposed integer parameter |
| manage_vfx: vfx_set_bool | no | Pipeline lacks setting VFX Graph exposed boolean parameter |
| manage_vfx: vfx_set_vector2 | no | Pipeline lacks setting VFX Graph Vector2 parameter |
| manage_vfx: vfx_set_vector3 | no | Pipeline lacks setting VFX Graph Vector3 parameter |
| manage_vfx: vfx_set_vector4 | no | Pipeline lacks setting VFX Graph Vector4 parameter |
| manage_vfx: vfx_set_color | no | Pipeline lacks setting VFX Graph Color parameter |
| manage_vfx: vfx_set_gradient | no | Pipeline lacks setting VFX Graph Gradient parameter |
| manage_vfx: vfx_set_texture | no | Pipeline lacks setting VFX Graph Texture parameter |
| manage_vfx: vfx_set_mesh | no | Pipeline lacks setting VFX Graph Mesh parameter |
| manage_vfx: vfx_set_curve | no | Pipeline lacks setting VFX Graph AnimationCurve parameter |
| manage_vfx: vfx_send_event | no | Pipeline lacks triggering VFX Graph custom event |
| manage_vfx: vfx_play | no | Pipeline lacks VisualEffect playback control |
| manage_vfx: vfx_stop | no | Pipeline lacks VisualEffect playback stop |
| manage_vfx: vfx_pause | no | Pipeline lacks VisualEffect playback pause |
| manage_vfx: vfx_reinit | no | Pipeline lacks VisualEffect re-initialization |
| manage_vfx: vfx_set_playback_speed | no | Pipeline lacks VisualEffect playback speed scaling |
| manage_vfx: vfx_set_seed | no | Pipeline lacks VisualEffect random seed configuration |
| manage_vfx: line_create | no | Pipeline lacks LineRenderer procedural shape generation (straight line, circle, arc, bezier) |
| manage_vfx: line_get_info | no | Pipeline lacks LineRenderer point array and styling inspection |
| manage_vfx: line_set_positions | no | Pipeline lacks LineRenderer position array assignment |
| manage_vfx: line_add_position | no | Pipeline lacks appending position to LineRenderer |
| manage_vfx: line_set_position | no | Pipeline lacks modifying single LineRenderer point index |
| manage_vfx: line_set_width | no | Pipeline lacks LineRenderer start/end width configuration |
| manage_vfx: line_set_color | no | Pipeline lacks LineRenderer gradient / color configuration |
| manage_vfx: line_set_material | no | Pipeline lacks LineRenderer material assignment |
| manage_vfx: line_set_properties | no | Pipeline lacks LineRenderer property block configuration |
| manage_vfx: line_clear | no | Pipeline lacks clearing LineRenderer points |
| manage_vfx: trail_get_info | no | Pipeline lacks TrailRenderer component inspection |
| manage_vfx: trail_set_time | no | Pipeline lacks TrailRenderer trail persistence duration configuration |
| manage_vfx: trail_set_width | no | Pipeline lacks TrailRenderer width curve configuration |
| manage_vfx: trail_set_color | no | Pipeline lacks TrailRenderer gradient configuration |
| manage_vfx: trail_set_material | no | Pipeline lacks TrailRenderer material assignment |
| manage_vfx: trail_set_properties | no | Pipeline lacks TrailRenderer property configuration |
| manage_vfx: trail_clear | no | Pipeline lacks clearing active TrailRenderer trails |
| manage_vfx: trail_emit | no | Pipeline lacks TrailRenderer emitting toggle |
| read_console: get | yes |  |
| read_console: clear | yes |  |
| refresh_unity | yes |  |
| run_tests | yes |  |
| script_apply_edits | no | Pipeline only writes raw text files; lacks AST-based script editing with symbol resolution |
| set_active_instance | yes |  |
| unity_docs: get_doc | no | Pipeline does not integrate live scraping/fetching of online Unity ScriptReference API documentation |
| unity_docs: get_manual | no | Pipeline does not fetch online Unity Manual pages |
| unity_docs: get_package_doc | no | Pipeline does not fetch package documentation from docs.unity3d.com |
| unity_docs: lookup | no | Pipeline lacks federated online documentation search across ScriptReference, Manual, and package docs |
| unity_reflect: get_type | yes |  |
| unity_reflect: get_member | yes |  |
| unity_reflect: search | yes |  |
| validate_script | no | Pipeline lacks in-memory script validation reporting Roslyn compiler diagnostics (errors/warnings) without writing to project |
