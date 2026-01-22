/**
 * dither.js - ASCII Dithering Effect
 * Creates a subtle ASCII art overlay for visual texture
 */

(function() {
    const ditherOverlay = document.querySelector('.dither-overlay');
    if (!ditherOverlay) {
        console.warn('Dither overlay element not found');
        return;
    }

    // ASCII characters for dithering (from light to dark)
    const ASCII_CHARS = [' ', '.', ':', '-', '=', '+', '*', '#', '%', '@'];
    const CHAR_SPACING = 8; // Character spacing in pixels
    
    // Generate ASCII pattern
    function generateDitherPattern() {
        const cols = Math.floor(window.innerWidth / CHAR_SPACING);
        const rows = Math.floor(window.innerHeight / CHAR_SPACING);
        
        let pattern = '';
        for (let y = 0; y < rows; y++) {
            for (let x = 0; x < cols; x++) {
                // Create a pattern based on position
                const noise = Math.sin(x * 0.5 + y * 0.3) * 0.5 + 0.5;
                const charIndex = Math.floor(noise * ASCII_CHARS.length);
                pattern += ASCII_CHARS[Math.min(charIndex, ASCII_CHARS.length - 1)];
            }
            pattern += '\n';
        }
        
        ditherOverlay.textContent = pattern;
    }

    // Generate pattern on load
    generateDitherPattern();
    
    // Regenerate on resize (debounced)
    let resizeTimeout;
    window.addEventListener('resize', function() {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(generateDitherPattern, 250);
    });
})();
