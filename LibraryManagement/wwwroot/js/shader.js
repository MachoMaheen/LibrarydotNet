/**
 * shader.js - WebGL Background Shaders
 * Features:
 * - "vibrant" mode: Colorful, moving gradient for the Home page.
 * - "greyscale" mode: Desaturated, slower movement for Auth pages (less distraction).
 */

const canvas = document.getElementById('bg-canvas');
if (!canvas) {
    console.warn('WebGL canvas not found - shader effects disabled');
} else {
    const gl = canvas.getContext('webgl');

    if (!gl) {
        console.error('WebGL not supported');
    } else {
        // --- Configuration ---
        // Determine mode based on body class or URL
        const isAuthPage = window.location.pathname.includes('login') || window.location.pathname.includes('register');
        const MODE = isAuthPage ? 'GREYSCALE' : 'VIBRANT';

        // --- Shaders ---

        const vertexShaderSource = `
            attribute vec2 position;
            void main() {
                gl_Position = vec4(position, 0.0, 1.0);
            }
        `;

        // Vibrant Shader (Home)
        const vibrantFragmentSource = `
            precision mediump float;
            uniform float time;
            uniform vec2 resolution;

            void main() {
                vec2 uv = gl_FragCoord.xy / resolution.xy;
                float t = time * 0.5;
                
                // Create moving colors using sine waves
                float r = 0.5 + 0.5 * sin(t + uv.x * 5.0);
                float g = 0.5 + 0.5 * sin(t * 0.8 + uv.y * 5.0 + 2.0);
                float b = 0.5 + 0.5 * sin(t * 1.2 + (uv.x + uv.y) * 3.0 + 4.0);
                
                // Soften colors (pastel-ish)
                vec3 color = vec3(r, g, b) * 0.4 + 0.3; 
                
                gl_FragColor = vec4(color, 1.0);
            }
        `;

        // Greyscale Shader (Auth - Less Intensive)
        const greyscaleFragmentSource = `
            precision mediump float;
            uniform float time;
            uniform vec2 resolution;

            void main() {
                vec2 uv = gl_FragCoord.xy / resolution.xy;
                float t = time * 0.2; // Slower movement
                
                // Simple moving gradient
                float val = 0.5 + 0.5 * sin(t + uv.x * 3.0 + uv.y * 2.0);
                
                // Map to light grey range
                float grey = 0.9 + 0.1 * val; 
                
                gl_FragColor = vec4(grey, grey, grey, 1.0);
            }
        `;

        const fragmentShaderSource = MODE === 'VIBRANT' ? vibrantFragmentSource : greyscaleFragmentSource;

        // --- Shader Compilation ---

        function createShader(gl, type, source) {
            const shader = gl.createShader(type);
            gl.shaderSource(shader, source);
            gl.compileShader(shader);
            if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
                console.error('Shader compile error:', gl.getShaderInfoLog(shader));
                gl.deleteShader(shader);
                return null;
            }
            return shader;
        }

        const vertexShader = createShader(gl, gl.VERTEX_SHADER, vertexShaderSource);
        const fragmentShader = createShader(gl, gl.FRAGMENT_SHADER, fragmentShaderSource);

        const program = gl.createProgram();
        gl.attachShader(program, vertexShader);
        gl.attachShader(program, fragmentShader);
        gl.linkProgram(program);

        if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
            console.error('Program link error:', gl.getProgramInfoLog(program));
        }

        // --- Buffers ---
        const positionBuffer = gl.createBuffer();
        gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
        // Full screen quad composed of two triangles
        const positions = [
            -1, -1,
            1, -1,
            -1, 1,
            -1, 1,
            1, -1,
            1, 1,
        ];
        gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(positions), gl.STATIC_DRAW);

        // --- Attributes & Uniforms ---
        const positionAttributeLocation = gl.getAttribLocation(program, "position");
        const timeUniformLocation = gl.getUniformLocation(program, "time");
        const resolutionUniformLocation = gl.getUniformLocation(program, "resolution");

        // --- Render Loop ---
        function resize() {
            canvas.width = window.innerWidth;
            canvas.height = window.innerHeight;
            gl.viewport(0, 0, canvas.width, canvas.height);
        }
        window.addEventListener('resize', resize);
        resize();

        function render(now) {
            now *= 0.001; // convert to seconds

            gl.useProgram(program);

            gl.enableVertexAttribArray(positionAttributeLocation);
            gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
            gl.vertexAttribPointer(positionAttributeLocation, 2, gl.FLOAT, false, 0, 0);

            gl.uniform1f(timeUniformLocation, now);
            gl.uniform2f(resolutionUniformLocation, canvas.width, canvas.height);

            gl.drawArrays(gl.TRIANGLES, 0, 6);

            requestAnimationFrame(render);
        }

        requestAnimationFrame(render);
    }
}
