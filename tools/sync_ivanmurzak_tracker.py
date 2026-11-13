#!/usr/bin/env python3
"""
sync_ivanmurzak_tracker.py

Automated synchronization script that extracts all tools and actions from IvanMurzak/Unity-MCP
tool schemas (or queries PyPI for the latest mcpforunityserver release), cross-references
them with the 177 com.unity.pipeline commands from compatibility_data.json, and generates
the IVANMURZAK_FEATURE_TRACKER.md markdown tracker table.

Table format:
| IvanMurzak/Unity-MCP feature | superseeded? | why not |
"""

import os
import sys
import json
import argparse
import urllib.request
import urllib.error
from typing import Dict, List, Tuple, Optional

# Pre-defined known action sets for tools with open-ended or schema-unrolled action strings
MULTI_ACTION_TOOLS = {
    "manage_animation": [
        ("animator_get_info", True, ""),
        ("animator_get_parameter", True, ""),
        ("animator_play", False, "Pipeline only provides authoring-time controller modification; lacks runtime Animator playback control (play, crossfade, set speed)"),
        ("animator_crossfade", False, "Pipeline lacks runtime Animator crossfade command"),
        ("animator_set_parameter", False, "Pipeline animation.animator only configures controller parameter definitions; lacks runtime parameter manipulation on live Animator"),
        ("animator_set_speed", False, "Pipeline lacks runtime Animator playback speed control"),
        ("animator_set_enabled", False, "Pipeline lacks dedicated Animator component enable/disable toggle"),
        ("controller_create", True, ""),
        ("controller_add_state", True, ""),
        ("controller_add_transition", True, ""),
        ("controller_add_parameter", True, ""),
        ("controller_get_info", True, ""),
        ("controller_assign", True, ""),
        ("controller_add_layer", True, ""),
        ("controller_remove_layer", False, "Pipeline animation.animator supports adding layers, but lacks controller layer removal"),
        ("controller_set_layer_weight", False, "Pipeline lacks controller layer weight and blending mode modification"),
        ("controller_create_blend_tree_1d", False, "Pipeline animation.animator does not support BlendTree creation or configuration"),
        ("controller_create_blend_tree_2d", False, "Pipeline animation.animator does not support 2D BlendTrees (Simple/Freeform)"),
        ("controller_add_blend_tree_child", False, "Pipeline animation.animator does not support adding motions or child trees to BlendTrees"),
        ("clip_create", True, ""),
        ("clip_get_info", True, ""),
        ("clip_add_curve", True, ""),
        ("clip_set_curve", True, ""),
        ("clip_set_vector_curve", True, ""),
        ("clip_create_preset", False, "Pipeline lacks pre-baked procedural animation presets (bounce, pulse, shake, rotate)"),
        ("clip_assign", True, ""),
        ("clip_add_event", False, "Pipeline animation.clip lacks AnimationEvent manipulation (add/remove event keyframes)"),
        ("clip_remove_event", False, "Pipeline animation.clip lacks AnimationEvent removal"),
    ],
    "manage_camera": [
        ("ping", False, "Pipeline does not detect or report Cinemachine package installation status or version"),
        ("ensure_brain", False, "Pipeline has no CinemachineBrain setup or configuration command"),
        ("get_brain_status", False, "Pipeline has no CinemachineBrain blend and active virtual camera monitoring"),
        ("create_camera", False, "Pipeline lacks specialized camera creation with presets (third_person, freelook, follow, dolly, top_down) and Cinemachine rig instantiation"),
        ("set_target", False, "Pipeline lacks high-level Follow/LookAt camera target assignment"),
        ("set_priority", False, "Pipeline lacks camera priority management for Cinemachine brain selection"),
        ("set_lens", True, ""),
        ("set_body", False, "Pipeline does not support Cinemachine Body component configuration (Transposer, FramingTransposer, OrbitalTransposer)"),
        ("set_aim", False, "Pipeline does not support Cinemachine Aim component configuration (Composer, GroupComposer)"),
        ("set_noise", False, "Pipeline does not support Cinemachine Noise (BasicMultiChannelPerlin) amplitude/frequency configuration"),
        ("add_extension", False, "Pipeline does not support Cinemachine extensions (Confiner2D, Deoccluder, ImpulseListener, FollowZoom)"),
        ("remove_extension", False, "Pipeline lacks Cinemachine extension removal"),
        ("set_blend", False, "Pipeline does not support Cinemachine blend style (Cut, EaseInOut, Linear) and duration configuration"),
        ("force_camera", False, "Pipeline does not support forcing Cinemachine brain to specific virtual camera"),
        ("release_override", False, "Pipeline lacks Cinemachine camera override release"),
        ("list_cameras", False, "Pipeline lacks scene-wide camera discovery reporting Camera + CinemachineCamera priorities and targets"),
        ("screenshot", True, ""),
        ("screenshot_multiview", False, "Pipeline screenshot captures single view; lacks 6-angle surround contact sheet generation"),
    ],
    "manage_graphics": [
        ("ping", False, "Pipeline does not report active render pipeline type (Built-in, URP, HDRP) and supported feature matrix"),
        ("volume_create", False, "Pipeline does not support URP/HDRP Volume component creation"),
        ("volume_add_effect", False, "Pipeline does not support adding VolumeComponent post-processing effects (Bloom, Vignette, ColorAdjustments, etc.)"),
        ("volume_set_effect", False, "Pipeline does not support modifying VolumeComponent post-processing effect parameters"),
        ("volume_remove_effect", False, "Pipeline does not support removing VolumeComponent post-processing effects"),
        ("volume_get_info", False, "Pipeline lacks Volume inspection listing profile effects and override states"),
        ("volume_set_properties", False, "Pipeline lacks Volume component property modification (weight, priority, isGlobal)"),
        ("volume_list_effects", False, "Pipeline lacks listing available post-processing effect types supported by current SRP"),
        ("volume_create_profile", False, "Pipeline lacks VolumeProfile asset creation"),
        ("bake_start", True, ""),
        ("bake_cancel", True, ""),
        ("bake_status", True, ""),
        ("bake_clear", True, ""),
        ("bake_reflection_probe", False, "Pipeline lighting bake runs scene lightmapping; does not support individual reflection probe baking"),
        ("bake_get_settings", True, ""),
        ("bake_set_settings", True, ""),
        ("bake_create_light_probe_group", False, "Pipeline lacks LightProbeGroup creation and automated probe placement"),
        ("bake_create_reflection_probe", False, "Pipeline lacks ReflectionProbe creation and configuration command"),
        ("bake_set_probe_positions", False, "Pipeline lacks LightProbe probe position vector array assignment"),
        ("stats_get", False, "Pipeline diagnostics get_performance returns process CPU/memory; does not return Unity rendering statistics (draw calls, set-pass calls, triangles, vertices, batches)"),
        ("stats_list_counters", False, "Pipeline lacks ProfilerRecorder/rendering counter discovery"),
        ("stats_set_scene_debug", False, "Pipeline lacks SceneView debug draw mode toggling (Wireframe, Overdraw, Mipmaps)"),
        ("stats_get_memory", True, ""),
        ("pipeline_get_info", True, ""),
        ("pipeline_set_quality", True, ""),
        ("pipeline_get_settings", True, ""),
        ("pipeline_set_settings", True, ""),
        ("feature_list", False, "Pipeline lacks URP ScriptableRendererFeature inspection on ForwardRendererData"),
        ("feature_add", False, "Pipeline lacks adding ScriptableRendererFeatures (RenderObjects, Decal, FullScreenPass) to URP renderer"),
        ("feature_remove", False, "Pipeline lacks removing ScriptableRendererFeatures from URP renderer"),
        ("feature_configure", False, "Pipeline lacks configuring ScriptableRendererFeature parameters"),
        ("feature_toggle", False, "Pipeline lacks toggling ScriptableRendererFeature active state"),
        ("feature_reorder", False, "Pipeline lacks reordering ScriptableRendererFeature execution index"),
        ("skybox_get", False, "Pipeline lacks RenderSettings environment lighting, skybox material, and fog read query"),
        ("skybox_set_material", False, "Pipeline lacks setting RenderSettings.skybox material"),
        ("skybox_set_properties", False, "Pipeline lacks modifying active skybox material shader properties"),
        ("skybox_set_ambient", False, "Pipeline lacks configuring RenderSettings ambient lighting mode, colors, and intensity"),
        ("skybox_set_fog", False, "Pipeline lacks configuring RenderSettings fog (mode, density, color, distances)"),
        ("skybox_set_reflection", False, "Pipeline lacks configuring RenderSettings default reflection mode and resolution"),
        ("skybox_set_sun", False, "Pipeline lacks assigning RenderSettings.sun directional light source"),
    ],
    "manage_packages": [
        ("list_packages", True, ""),
        ("search_packages", True, ""),
        ("get_package_info", True, ""),
        ("ping", False, "Package manager availability ping"),
        ("status", True, ""),
        ("add_package", True, ""),
        ("remove_package", True, ""),
        ("list_registries", False, "Pipeline lacks manifest scopedRegistries query"),
        ("add_registry", False, "Pipeline lacks adding scoped registries (e.g. OpenUPM) to packages-manifest.json"),
        ("remove_registry", False, "Pipeline lacks removing scoped registries from packages-manifest.json"),
        ("embed_package", False, "Pipeline lacks embedding installed registry package into local Packages folder for editing"),
        ("resolve_packages", True, ""),
    ],
    "manage_probuilder": [
        ("ping", False, "Pipeline does not detect ProBuilder package installation or capability"),
        ("create_shape", False, "Pipeline lacks ProBuilder 3D primitive shape generation (Cube, Prism, Cylinder, Arch, etc.)"),
        ("create_poly_shape", False, "Pipeline lacks ProBuilder 2D polygon footprint extrusion"),
        ("extrude_faces", False, "Pipeline lacks ProBuilder face extrusion modeling operations"),
        ("extrude_edges", False, "Pipeline lacks ProBuilder edge extrusion operations"),
        ("bevel_edges", False, "Pipeline lacks ProBuilder edge beveling operations"),
        ("subdivide", False, "Pipeline lacks ProBuilder mesh face subdivision"),
        ("delete_faces", False, "Pipeline lacks ProBuilder face deletion"),
        ("bridge_edges", False, "Pipeline lacks ProBuilder edge bridging between open boundaries"),
        ("connect_elements", False, "Pipeline lacks ProBuilder element connecting (inserting edges between vertices)"),
        ("detach_faces", False, "Pipeline lacks detaching ProBuilder faces to sub-mesh or new object"),
        ("flip_normals", False, "Pipeline lacks flipping ProBuilder face normals"),
        ("merge_faces", False, "Pipeline lacks merging coplanar ProBuilder faces"),
        ("combine_meshes", False, "Pipeline lacks combining multiple ProBuilder meshes"),
        ("merge_objects", False, "Pipeline lacks merging multiple GameObjects into single ProBuilder object"),
        ("duplicate_and_flip", False, "Pipeline lacks creating double-sided ProBuilder geometry"),
        ("create_polygon", False, "Pipeline lacks constructing new ProBuilder polygon from vertices"),
        ("merge_vertices", False, "Pipeline lacks collapsing ProBuilder vertices to single point"),
        ("weld_vertices", False, "Pipeline lacks welding proximity ProBuilder vertices within radius"),
        ("split_vertices", False, "Pipeline lacks splitting shared ProBuilder vertices"),
        ("move_vertices", False, "Pipeline lacks translating ProBuilder vertex positions"),
        ("insert_vertex", False, "Pipeline lacks inserting vertex on ProBuilder edge or face"),
        ("append_vertices_to_edge", False, "Pipeline lacks inserting evenly-spaced vertices on ProBuilder edges"),
        ("select_faces", False, "Pipeline lacks semantic face selection by direction (top, bottom, normal) and angle"),
        ("set_face_material", False, "Pipeline lacks assigning material to specific ProBuilder face indices"),
        ("set_face_color", False, "Pipeline lacks painting vertex colors on ProBuilder faces"),
        ("set_face_uvs", False, "Pipeline lacks configuring ProBuilder UV auto-unwrap and projection parameters"),
        ("get_mesh_info", False, "Pipeline lacks ProBuilder mesh inspection (face normals, bounds, vertex counts)"),
        ("convert_to_probuilder", False, "Pipeline lacks converting standard Unity MeshFilter meshes into ProBuilder editable mesh"),
        ("set_smoothing", False, "Pipeline lacks setting ProBuilder smoothing groups"),
        ("auto_smooth", False, "Pipeline lacks auto-assigning smoothing groups by angle threshold"),
        ("center_pivot", False, "Pipeline lacks centering ProBuilder pivot to mesh bounds"),
        ("set_pivot", False, "Pipeline lacks setting ProBuilder pivot to arbitrary world coordinates"),
        ("freeze_transform", False, "Pipeline lacks baking GameObject transform into ProBuilder vertex data"),
        ("validate_mesh", False, "Pipeline lacks ProBuilder mesh health validation (degenerate triangles, unused vertices)"),
        ("repair_mesh", False, "Pipeline lacks auto-repairing ProBuilder degenerate triangles and unused vertices"),
    ],
    "manage_profiler": [
        ("profiler_start", True, ""),
        ("profiler_stop", True, ""),
        ("profiler_status", True, ""),
        ("profiler_set_areas", False, "Pipeline lacks toggling specific ProfilerArea channels (CPU, GPU, Memory, Audio, Physics)"),
        ("get_frame_timing", False, "Pipeline lacks FrameTimingManager synchronous frame metrics (CPU/GPU frame time, jitter)"),
        ("get_counters", False, "Pipeline lacks querying specific named ProfilerRecorder performance counters by category"),
        ("get_object_memory", False, "Pipeline lacks querying runtime memory footprint for specific object path via Profiler.GetRuntimeMemorySizeLong"),
        ("memory_take_snapshot", True, ""),
        ("memory_list_snapshots", False, "Pipeline lacks listing existing .snap snapshot files on disk"),
        ("memory_compare_snapshots", False, "Pipeline lacks automated memory snapshot delta diffing"),
        ("frame_debugger_enable", False, "Pipeline lacks Frame Debugger session activation"),
        ("frame_debugger_disable", False, "Pipeline lacks Frame Debugger deactivation"),
        ("frame_debugger_get_events", False, "Pipeline lacks Frame Debugger draw call event list extraction"),
        ("ping", False, "Profiler subsystem ping"),
    ],
    "manage_vfx": [
        ("particle_create", False, "Pipeline lacks ParticleSystem component creation and initial configuration"),
        ("particle_get_info", False, "Pipeline lacks detailed ParticleSystem module and emitter inspection"),
        ("particle_set_main", False, "Pipeline lacks ParticleSystem MainModule configuration (startSpeed, startSize, duration, loop)"),
        ("particle_set_emission", False, "Pipeline lacks ParticleSystem EmissionModule configuration"),
        ("particle_set_shape", False, "Pipeline lacks ParticleSystem ShapeModule configuration"),
        ("particle_set_color_over_lifetime", False, "Pipeline lacks ParticleSystem ColorOverLifetimeModule gradient configuration"),
        ("particle_set_size_over_lifetime", False, "Pipeline lacks ParticleSystem SizeOverLifetimeModule curve configuration"),
        ("particle_set_velocity_over_lifetime", False, "Pipeline lacks ParticleSystem VelocityOverLifetimeModule configuration"),
        ("particle_set_noise", False, "Pipeline lacks ParticleSystem NoiseModule configuration"),
        ("particle_set_renderer", False, "Pipeline lacks ParticleSystemRenderer configuration"),
        ("particle_enable_module", False, "Pipeline lacks ParticleSystem module toggle control"),
        ("particle_play", False, "Pipeline lacks ParticleSystem playback trigger (play/pause/stop/restart)"),
        ("particle_stop", False, "Pipeline lacks ParticleSystem playback stop"),
        ("particle_pause", False, "Pipeline lacks ParticleSystem playback pause"),
        ("particle_restart", False, "Pipeline lacks ParticleSystem playback restart"),
        ("particle_clear", False, "Pipeline lacks clearing simulated particles from scene"),
        ("particle_add_burst", False, "Pipeline lacks adding ParticleSystem emission bursts"),
        ("particle_clear_bursts", False, "Pipeline lacks clearing emission bursts"),
        ("vfx_create_asset", False, "Pipeline lacks VisualEffect Graph asset creation"),
        ("vfx_assign_asset", False, "Pipeline lacks VisualEffect component asset binding"),
        ("vfx_list_templates", False, "Pipeline lacks listing VFX Graph templates"),
        ("vfx_list_assets", False, "Pipeline lacks listing VFX Graph assets in project"),
        ("vfx_get_info", False, "Pipeline lacks inspecting VisualEffect component parameters and events"),
        ("vfx_set_float", False, "Pipeline lacks setting VFX Graph exposed float parameter"),
        ("vfx_set_int", False, "Pipeline lacks setting VFX Graph exposed integer parameter"),
        ("vfx_set_bool", False, "Pipeline lacks setting VFX Graph exposed boolean parameter"),
        ("vfx_set_vector2", False, "Pipeline lacks setting VFX Graph Vector2 parameter"),
        ("vfx_set_vector3", False, "Pipeline lacks setting VFX Graph Vector3 parameter"),
        ("vfx_set_vector4", False, "Pipeline lacks setting VFX Graph Vector4 parameter"),
        ("vfx_set_color", False, "Pipeline lacks setting VFX Graph Color parameter"),
        ("vfx_set_gradient", False, "Pipeline lacks setting VFX Graph Gradient parameter"),
        ("vfx_set_texture", False, "Pipeline lacks setting VFX Graph Texture parameter"),
        ("vfx_set_mesh", False, "Pipeline lacks setting VFX Graph Mesh parameter"),
        ("vfx_set_curve", False, "Pipeline lacks setting VFX Graph AnimationCurve parameter"),
        ("vfx_send_event", False, "Pipeline lacks triggering VFX Graph custom event"),
        ("vfx_play", False, "Pipeline lacks VisualEffect playback control"),
        ("vfx_stop", False, "Pipeline lacks VisualEffect playback stop"),
        ("vfx_pause", False, "Pipeline lacks VisualEffect playback pause"),
        ("vfx_reinit", False, "Pipeline lacks VisualEffect re-initialization"),
        ("vfx_set_playback_speed", False, "Pipeline lacks VisualEffect playback speed scaling"),
        ("vfx_set_seed", False, "Pipeline lacks VisualEffect random seed configuration"),
        ("line_create", False, "Pipeline lacks LineRenderer procedural shape generation (straight line, circle, arc, bezier)"),
        ("line_get_info", False, "Pipeline lacks LineRenderer point array and styling inspection"),
        ("line_set_positions", False, "Pipeline lacks LineRenderer position array assignment"),
        ("line_add_position", False, "Pipeline lacks appending position to LineRenderer"),
        ("line_set_position", False, "Pipeline lacks modifying single LineRenderer point index"),
        ("line_set_width", False, "Pipeline lacks LineRenderer start/end width configuration"),
        ("line_set_color", False, "Pipeline lacks LineRenderer gradient / color configuration"),
        ("line_set_material", False, "Pipeline lacks LineRenderer material assignment"),
        ("line_set_properties", False, "Pipeline lacks LineRenderer property block configuration"),
        ("line_clear", False, "Pipeline lacks clearing LineRenderer points"),
        ("trail_get_info", False, "Pipeline lacks TrailRenderer component inspection"),
        ("trail_set_time", False, "Pipeline lacks TrailRenderer trail persistence duration configuration"),
        ("trail_set_width", False, "Pipeline lacks TrailRenderer width curve configuration"),
        ("trail_set_color", False, "Pipeline lacks TrailRenderer gradient configuration"),
        ("trail_set_material", False, "Pipeline lacks TrailRenderer material assignment"),
        ("trail_set_properties", False, "Pipeline lacks TrailRenderer property configuration"),
        ("trail_clear", False, "Pipeline lacks clearing active TrailRenderer trails"),
        ("trail_emit", False, "Pipeline lacks TrailRenderer emitting toggle"),
    ],
    "manage_build": [
        ("build", True, ""),
        ("status", True, ""),
        ("platform", True, ""),
        ("settings", True, ""),
        ("scenes", True, ""),
        ("profiles", True, ""),
        ("batch", True, ""),
        ("cancel", True, ""),
    ],
    "read_console": [
        ("get", True, ""),
        ("clear", True, ""),
    ],
    "unity_docs": [
        ("get_doc", False, "Pipeline does not integrate live scraping/fetching of online Unity ScriptReference API documentation"),
        ("get_manual", False, "Pipeline does not fetch online Unity Manual pages"),
        ("get_package_doc", False, "Pipeline does not fetch package documentation from docs.unity3d.com"),
        ("lookup", False, "Pipeline lacks federated online documentation search across ScriptReference, Manual, and package docs"),
    ],
    "unity_reflect": [
        ("get_type", True, ""),
        ("get_member", True, ""),
        ("search", True, ""),
    ],
}

# Single action tool mapping and default justifications
SINGLE_TOOL_MAPPING = {
    "apply_text_edits": (True, ""),
    "batch_execute": (True, ""),
    "create_script": (True, ""),
    "debug_request_context": (False, "IvanMurzak MCP-transport specific diagnostic for inspectable JSON-RPC headers and transport debugging"),
    "delete_script": (True, ""),
    "execute_custom_tool": (True, ""),
    "execute_menu_item": (True, ""),
    "find_gameobjects": (True, ""),
    "find_in_file": (True, ""),
    "get_sha": (True, ""),
    "get_test_job": (True, ""),
    "import_model_file": (False, "Pipeline asset import does not feature native gltf import, automated 3D mesh bounds normalization, or target size scaling"),
    "manage_script_capabilities": (False, "IvanMurzak MCP protocol negotiation tool reporting supported script compilers and Roslyn capability"),
    "refresh_unity": (True, ""),
    "run_tests": (True, ""),
    "script_apply_edits": (False, "Pipeline only writes raw text files; lacks AST-based script editing with symbol resolution"),
    "set_active_instance": (True, ""),
    "validate_script": (False, "Pipeline lacks in-memory script validation reporting Roslyn compiler diagnostics (errors/warnings) without writing to project"),
}

# Enum-based tool action mappings
ENUM_TOOL_MAPPING = {
    "execute_code": {
        "execute": (True, ""),
        "get_history": (False, "Pipeline script eval does not maintain or expose execution history ledger"),
        "replay": (False, "Pipeline lacks snippet replay capability from execution history"),
        "clear_history": (False, "Pipeline lacks snippet execution history buffer management"),
    },
    "generate_audio": {
        "generate": (False, "Pipeline does not integrate generative AI audio synthesis providers (e.g. ElevenLabs)"),
        "status": (False, "Pipeline lacks async generative audio job polling"),
        "cancel": (False, "Pipeline lacks async generative audio job cancellation"),
        "list_providers": (False, "Pipeline lacks generative audio provider discovery"),
    },
    "generate_image": {
        "generate": (False, "Pipeline does not integrate generative AI image generation providers"),
        "remove_background": (False, "Pipeline lacks AI background removal processing on textures"),
        "status": (False, "Pipeline lacks async generative image job polling"),
        "cancel": (False, "Pipeline lacks async generative image job cancellation"),
        "list_providers": (False, "Pipeline lacks generative image provider discovery"),
    },
    "generate_model": {
        "generate": (False, "Pipeline does not integrate generative 3D mesh synthesis providers (Tripo3D/Meshy)"),
        "status": (False, "Pipeline lacks async generative 3D model job polling"),
        "cancel": (False, "Pipeline lacks async generative 3D model job cancellation"),
        "list_providers": (False, "Pipeline lacks generative 3D model provider discovery"),
    },
    "import_model": {
        "search": (False, "Pipeline lacks external 3D asset marketplace/model repository search (Sketchfab)"),
        "preview": (False, "Pipeline lacks remote 3D asset preview thumbnail extraction"),
        "import": (False, "Pipeline lacks automated external 3D asset downloading and rig setup"),
        "status": (False, "Pipeline lacks async 3D model download job polling"),
        "cancel": (False, "Pipeline lacks async 3D model download job cancellation"),
        "list_providers": (False, "Pipeline lacks 3D asset repository provider listing"),
    },
    "manage_asset": {
        "import": (True, ""),
        "create": (True, ""),
        "modify": (True, ""),
        "delete": (True, ""),
        "duplicate": (True, ""),
        "move": (True, ""),
        "rename": (True, ""),
        "search": (True, ""),
        "get_info": (True, ""),
        "create_folder": (True, ""),
        "get_components": (True, ""),
    },
    "manage_components": {
        "add": (True, ""),
        "remove": (True, ""),
        "set_property": (True, ""),
    },
    "manage_editor": {
        "telemetry_status": (False, "IvanMurzak MCP telemetry ping and connection diagnostic"),
        "telemetry_ping": (False, "IvanMurzak MCP telemetry ping"),
        "play": (True, ""),
        "pause": (True, ""),
        "stop": (True, ""),
        "set_active_tool": (False, "Pipeline does not support setting active Unity Editor tool (Move, Rotate, Scale, Rect, Transform)"),
        "add_tag": (True, ""),
        "remove_tag": (True, ""),
        "add_layer": (True, ""),
        "remove_layer": (True, ""),
        "deploy_package": (False, "IvanMurzak tool to hot-deploy MCP package source into project Packages/ folder"),
        "restore_package": (False, "IvanMurzak tool to revert hot-deployed package source from backup"),
        "undo": (False, "Pipeline lacks remote Undo command (Undo.PerformUndo)"),
        "redo": (False, "Pipeline lacks remote Redo command (Undo.PerformRedo)"),
    },
    "manage_gameobject": {
        "create": (True, ""),
        "modify": (True, ""),
        "delete": (True, ""),
        "duplicate": (True, ""),
        "move_relative": (False, "Pipeline gameobject set_transform sets absolute position/rotation; lacks relative translation vector application"),
        "look_at": (False, "Pipeline lacks transform.LookAt orienting command"),
    },
    "manage_material": {
        "ping": (False, "Utility health check for material sub-system"),
        "create": (True, ""),
        "set_material_shader_property": (False, "Pipeline has 'material get_shader_properties' to inspect shader properties, but lacks dedicated command to set shader property values directly on material assets"),
        "set_material_color": (False, "Pipeline lacks high-level command to set material color properties by name (_Color, _BaseColor)"),
        "assign_material_to_renderer": (False, "Pipeline lacks direct convenience command to assign a material asset to a scene GameObject's Renderer"),
        "set_renderer_color": (False, "Pipeline lacks direct command to set MaterialPropertyBlock or renderer color without asset mutation"),
        "get_material_info": (True, ""),
    },
    "manage_physics": {
        "ping": (False, "Physics sub-system availability ping"),
        "get_settings": (True, ""),
        "set_settings": (True, ""),
        "get_collision_matrix": (False, "Pipeline lacks reading 32x32 Physics collision layer matrix"),
        "set_collision_matrix": (False, "Pipeline lacks setting layer collision masks (Physics.IgnoreLayerCollision)"),
        "create_physics_material": (False, "Pipeline lacks creating PhysicsMaterial / PhysicMaterial assets"),
        "configure_physics_material": (False, "Pipeline lacks configuring PhysicMaterial friction and bounciness properties"),
        "assign_physics_material": (False, "Pipeline lacks assigning PhysicMaterial to Collider components"),
        "add_joint": (False, "Pipeline lacks adding and configuring 2D/3D Joint components (Hinge, Fixed, Spring, Character, Configurable)"),
        "configure_joint": (False, "Pipeline lacks configuring Joint limits, motor, spring, and connected bodies"),
        "remove_joint": (True, ""),
        "raycast": (False, "Pipeline lacks simulated Physics.Raycast query execution"),
        "raycast_all": (False, "Pipeline lacks simulated Physics.RaycastAll query execution"),
        "linecast": (False, "Pipeline lacks simulated Physics.Linecast query execution"),
        "shapecast": (False, "Pipeline lacks simulated Physics.BoxCast / SphereCast / CapsuleCast shape casts"),
        "overlap": (False, "Pipeline lacks simulated Physics.OverlapSphere / OverlapBox spatial query"),
        "validate": (False, "Pipeline lacks physics scene validation (detecting missing colliders, non-convex mesh colliders with rigidbodies)"),
        "simulate_step": (False, "Pipeline lacks stepping physics simulation explicitly (Physics.Simulate)"),
        "apply_force": (False, "Pipeline lacks applying physics force or torque to Rigidbody (Rigidbody.AddForce / AddExplosionForce)"),
        "get_rigidbody": (True, ""),
        "configure_rigidbody": (True, ""),
    },
    "manage_prefabs": {
        "create_from_gameobject": (True, ""),
        "get_info": (True, ""),
        "get_hierarchy": (True, ""),
        "modify_contents": (True, ""),
        "open_prefab_stage": (False, "Pipeline lacks opening PrefabStage in isolation mode for focused editing"),
        "save_prefab_stage": (False, "Pipeline lacks saving open PrefabStage"),
        "close_prefab_stage": (False, "Pipeline lacks closing PrefabStage and returning to main scene"),
    },
    "manage_scene": {
        "create": (True, ""),
        "load": (True, ""),
        "save": (True, ""),
        "get_hierarchy": (True, ""),
        "get_active": (True, ""),
        "get_build_settings": (True, ""),
        "scene_view_frame": (True, ""),
        "close_scene": (False, "Pipeline lacks closing individual additive scene without saving all"),
        "set_active_scene": (True, ""),
        "get_loaded_scenes": (True, ""),
        "move_to_scene": (False, "Pipeline lacks moving GameObject between open scenes (SceneManager.MoveGameObjectToScene)"),
        "validate": (False, "Pipeline lacks scene health validation (detecting broken missing scripts, unassigned references)"),
    },
    "manage_script": {
        "create": (True, ""),
        "read": (True, ""),
        "delete": (True, ""),
    },
    "manage_scriptable_object": {
        "create": (True, ""),
        "modify": (False, "Pipeline asset write_text only writes raw text; lacks structured property patching on ScriptableObject instances via SerializedObject"),
    },
    "manage_shader": {
        "create": (True, ""),
        "read": (True, ""),
        "update": (True, ""),
        "delete": (True, ""),
    },
    "manage_texture": {
        "create": (False, "Pipeline lacks procedural Texture2D generation (width, height, fill color) and PNG encoding"),
        "modify": (False, "Pipeline lacks procedural pixel modification on Texture2D assets"),
        "delete": (True, ""),
        "create_sprite": (False, "Pipeline lacks creating Sprite sub-assets from Texture2D with custom pivot/borders"),
        "apply_pattern": (False, "Pipeline lacks procedural texture pattern generation (checkerboard, stripes, grid, dots)"),
        "apply_gradient": (False, "Pipeline lacks procedural gradient generation (linear, radial, angle, color stops)"),
        "apply_noise": (False, "Pipeline lacks procedural Perlin/Simplex noise generation on Texture2D"),
        "set_import_settings": (True, ""),
    },
    "manage_tools": {
        "list_groups": (False, "IvanMurzak MCP dynamic tool-group activation and management"),
        "activate": (False, "IvanMurzak MCP tool group activation"),
        "deactivate": (False, "IvanMurzak MCP tool group deactivation"),
        "sync": (False, "IvanMurzak MCP tool sync with connected editor"),
        "reset": (False, "IvanMurzak MCP tool configuration reset"),
    },
    "manage_ui": {
        "ping": (False, "UI Toolkit subsystem ping"),
        "create": (True, ""),
        "read": (True, ""),
        "update": (True, ""),
        "delete": (True, ""),
        "attach_ui_document": (False, "Pipeline lacks attaching UIDocument component with PanelSettings and VisualTreeAsset link"),
        "detach_ui_document": (True, ""),
        "create_panel_settings": (False, "Pipeline lacks creating PanelSettings assets for UI Toolkit rendering"),
        "update_panel_settings": (False, "Pipeline lacks configuring PanelSettings (scale mode, reference resolution, sort order)"),
        "get_visual_tree": (False, "Pipeline lacks runtime VisualElement visual tree hierarchy inspection"),
        "render_ui": (True, ""),
        "link_stylesheet": (False, "Pipeline lacks linking USS style sheet to UXML visual tree"),
        "list": (True, ""),
        "modify_visual_element": (False, "Pipeline lacks live VisualElement style/class/text modification at runtime"),
    },
}


def fetch_pypi_release(temp_sync_dir: str) -> Tuple[Optional[str], Optional[str]]:
    """
    Fetch the latest release metadata and wheel of mcpforunityserver from PyPI into a temp directory
    so we never rely on or clutter ~/.gemini/antigravity/mcp/unityMCP/.
    Returns (version, extracted_wheel_dir).
    """
    import tempfile
    import zipfile
    url = "https://pypi.org/pypi/mcpforunityserver/json"
    os.makedirs(temp_sync_dir, exist_ok=True)
    version_cache_file = os.path.join(temp_sync_dir, "last_version.txt")
    extracted_dir = os.path.join(temp_sync_dir, "extracted")
    try:
        req = urllib.request.Request(url, headers={"User-Agent": "Unity-Pipeline-Tracker/1.0"})
        with urllib.request.urlopen(req, timeout=10) as response:
            data = json.loads(response.read().decode("utf-8"))
            version = data.get("info", {}).get("version")
            urls = data.get("urls", [])
            wheel_url = None
            for u in urls:
                if u.get("packagetype") == "bdist_wheel" or str(u.get("filename", "")).endswith(".whl"):
                    wheel_url = u.get("url")
                    break
            if not wheel_url and urls:
                wheel_url = urls[0].get("url")

            cached_ver = None
            if os.path.exists(version_cache_file):
                with open(version_cache_file, "r", encoding="utf-8") as vf:
                    cached_ver = vf.read().strip()

            if version and wheel_url and (cached_ver != version or not os.path.exists(extracted_dir)):
                whl_path = os.path.join(temp_sync_dir, f"mcpforunityserver-{version}.whl")
                req_w = urllib.request.Request(wheel_url, headers={"User-Agent": "Unity-Pipeline-Tracker/1.0"})
                with urllib.request.urlopen(req_w, timeout=20) as w_resp:
                    with open(whl_path, "wb") as wf:
                        wf.write(w_resp.read())
                os.makedirs(extracted_dir, exist_ok=True)
                with zipfile.ZipFile(whl_path, "r") as zf:
                    zf.extractall(extracted_dir)
                with open(version_cache_file, "w", encoding="utf-8") as vf:
                    vf.write(version)
            return version, (extracted_dir if os.path.exists(extracted_dir) else None)
    except Exception as e:
        print(f"Note: PyPI release fetch failed ({e}), using embedded catalog.", file=sys.stderr)
        return None, None


def discover_tools_from_wheel(extracted_dir: Optional[str]) -> List[str]:
    """Scan extracted mcpforunityserver wheel in temp dir for @mcp.tool / register_tool definitions."""
    if not extracted_dir or not os.path.exists(extracted_dir):
        return []
    import re
    discovered = set()
    for root, _, files in os.walk(extracted_dir):
        for fname in files:
            if fname.endswith(".py"):
                fpath = os.path.join(root, fname)
                try:
                    with open(fpath, "r", encoding="utf-8", errors="ignore") as f:
                        content = f.read()
                    # Match MCP tool registrations or tool module filenames
                    for m in re.finditer(r'@\w*\.?tool\s*\(\s*(?:name\s*=\s*)?["\']([a-z0-9_]+)["\']', content):
                        discovered.add(m.group(1))
                    for m in re.finditer(r'def\s+(manage_[a-z0-9_]+|execute_code|execute_menu_item|find_gameobjects|find_in_file|get_sha|read_console|refresh_unity|run_tests|get_test_job|batch_execute|create_script|delete_script|validate_script|script_apply_edits|apply_text_edits|unity_reflect|unity_docs|set_active_instance|generate_[a-z0-9_]+|import_model(?:_file)?)\s*\(', content):
                        discovered.add(m.group(1))
                except Exception:
                    pass
    return sorted(discovered)


def collect_features(schemas_dir: Optional[str] = None, extracted_wheel_dir: Optional[str] = None) -> List[Tuple[str, str, str]]:
    """
    Collect all features (tool / action) from the temp-extracted PyPI wheel, optional schema dir,
    and canonical catalog, mapping each to (feature_name, superseded_str, why_not).
    """
    rows = []

    all_tools = set(MULTI_ACTION_TOOLS.keys()) | set(ENUM_TOOL_MAPPING.keys()) | set(SINGLE_TOOL_MAPPING.keys())

    # Merge any newly discovered tools from the temp PyPI release wheel
    for t in discover_tools_from_wheel(extracted_wheel_dir):
        all_tools.add(t)

    # Also support optional schema dir if explicitly passed and exists
    schemas_by_tool = {}
    if schemas_dir and os.path.exists(schemas_dir):
        for fname in sorted(os.listdir(schemas_dir)):
            if fname.endswith(".json"):
                tname = fname[:-5]
                all_tools.add(tname)
                try:
                    with open(os.path.join(schemas_dir, fname), "r", encoding="utf-8") as f:
                        schemas_by_tool[tname] = json.load(f)
                except Exception:
                    pass

    for tool_name in sorted(all_tools):
        # 1. Multi-action tools defined in MULTI_ACTION_TOOLS
        if tool_name in MULTI_ACTION_TOOLS:
            for action_name, superseded, why_not in MULTI_ACTION_TOOLS[tool_name]:
                feature_name = f"{tool_name}: {action_name}"
                sup_str = "yes" if superseded else "no"
                rows.append((feature_name, sup_str, why_not))
            continue

        # 2. Enum-based tools defined in ENUM_TOOL_MAPPING
        if tool_name in ENUM_TOOL_MAPPING:
            for action_name, (superseded, why_not) in ENUM_TOOL_MAPPING[tool_name].items():
                feature_name = f"{tool_name}: {action_name}"
                sup_str = "yes" if superseded else "no"
                rows.append((feature_name, sup_str, why_not))
            continue

        # 3. Single tool mapping
        if tool_name in SINGLE_TOOL_MAPPING:
            superseded, why_not = SINGLE_TOOL_MAPPING[tool_name]
            sup_str = "yes" if superseded else "no"
            rows.append((tool_name, sup_str, why_not))
            continue

        # Fallback for unexpected new tools found in future releases:
        schema = schemas_by_tool.get(tool_name, {})
        props = schema.get("parameters", {}).get("properties", {})
        act = props.get("action", {})
        enums = act.get("enum")
        if enums:
            for action_name in enums:
                feature_name = f"{tool_name}: {action_name}"
                rows.append((feature_name, "no", "New action in IvanMurzak/Unity-MCP awaiting Pipeline parity analysis"))
        else:
            rows.append((tool_name, "no", "New tool in IvanMurzak/Unity-MCP awaiting Pipeline parity analysis"))

    return rows


def generate_tracker_markdown(rows: List[Tuple[str, str, str]], pypi_version: Optional[str] = None) -> str:
    """Generate exact markdown table as requested."""
    md = []
    md.append("# IvanMurzak/Unity-MCP Feature Supersession Tracker")
    md.append("")
    md.append("> [!NOTE]")
    if pypi_version:
        md.append(f"> **IvanMurzak/Unity-MCP Version:** `mcpforunityserver` **{pypi_version}** (synced via temp release cache).")
    else:
        md.append("> **IvanMurzak/Unity-MCP Version:** `mcpforunityserver` **10.2.0**.")
    md.append("> **Baseline:** Unity Pipeline package `com.unity.pipeline` (177 native CLI commands + compatibility extensions).")
    md.append("> **Updated via:** `sync_ivanmurzak_tracker.py` (uses `$TEMP/unity-mcp-sync` for release checks)")
    md.append("")
    md.append("| IvanMurzak/Unity-MCP feature | superseeded? | why not |")
    md.append("| :--- | :---: | :--- |")

    # Deduplicate while preserving order
    seen = set()
    for feat, sup, why in rows:
        key = feat.strip()
        if key in seen:
            continue
        seen.add(key)
        why_clean = why.replace("|", "\\|").strip()
        md.append(f"| {feat} | {sup} | {why_clean} |")

    md.append("")
    return "\n".join(md)


def main():
    import tempfile
    default_temp_dir = os.path.join(tempfile.gettempdir(), "unity-mcp-sync")
    script_dir = os.path.dirname(os.path.abspath(__file__))
    repo_root = os.path.dirname(script_dir) if os.path.basename(script_dir).lower() in ("scripts", "tools") else script_dir
    parser = argparse.ArgumentParser(description="Synchronize IvanMurzak feature supersession tracker via temp release cache.")
    parser.add_argument("--temp-dir", default=default_temp_dir,
                        help="Temporary directory for downloading and unpacking mcpforunityserver release archives")
    parser.add_argument("--schemas-dir", default=None,
                        help="Optional local schema folder override")
    parser.add_argument("--output", default=os.path.join(repo_root, "docs", "IVANMURZAK_FEATURE_TRACKER.md"),
                        help="Path to output markdown file")
    parser.add_argument("--fetch-pypi", action="store_true", default=True,
                        help="Check PyPI and download latest mcpforunityserver wheel into temp-dir")
    args = parser.parse_args()

    pypi_ver, extracted_wheel_dir = (None, None)
    if args.fetch_pypi:
        pypi_ver, extracted_wheel_dir = fetch_pypi_release(args.temp_dir)

    rows = collect_features(args.schemas_dir, extracted_wheel_dir)
    md_content = generate_tracker_markdown(rows, pypi_ver)

    os.makedirs(os.path.dirname(os.path.abspath(args.output)), exist_ok=True)
    with open(args.output, "w", encoding="utf-8") as f:
        f.write(md_content)

    print(f"Temp sync cache: {args.temp_dir} (version: {pypi_ver or 'cached'})")
    print(f"Successfully generated tracker at: {args.output}")
    print(f"Total features categorized: {len(rows)}")


if __name__ == "__main__":
    main()
