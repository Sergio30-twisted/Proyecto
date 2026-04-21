(function () {
    const particleCount = 70; // Cantidad de puntos
    let canvas, ctx, particles = [];

    function init() {
        canvas = document.getElementById('canvas-home');
        if (!canvas) return;

        ctx = canvas.getContext('2d');

        // Ajustamos el tamaño inicial y creamos partículas
        resize();
        animate();

        window.addEventListener('resize', resize);
    }

    class Particle {
        constructor() {
            this.x = Math.random() * canvas.width;
            this.y = Math.random() * canvas.height;
            // Velocidad aleatoria
            this.vx = (Math.random() - 0.5) * 0.4;
            this.vy = (Math.random() - 0.5) * 0.4;
            this.size = 2;
        }

        update() {
            this.x += this.vx;
            this.y += this.vy;

            // Rebote en los bordes del banner
            if (this.x < 0 || this.x > canvas.width) this.vx *= -1;
            if (this.y < 0 || this.y > canvas.height) this.vy *= -1;
        }

        draw() {
            ctx.fillStyle = '#00eaff'; // El cian de tu diseño
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
            ctx.fill();
        }
    }

    function resize() {
        // Obligamos al canvas a medir lo que mide su contenedor padre
        canvas.width = canvas.parentElement.clientWidth;
        canvas.height = canvas.parentElement.clientHeight;

        // Reiniciamos partículas al cambiar tamaño para cubrir todo el espacio
        particles = [];
        for (let i = 0; i < particleCount; i++) {
            particles.push(new Particle());
        }
    }

    function animate() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        particles.forEach((p, index) => {
            p.update();
            p.draw();

            // Dibujamos líneas entre puntos cercanos
            for (let j = index + 1; j < particles.length; j++) {
                const p2 = particles[j];
                const dist = Math.hypot(p.x - p2.x, p.y - p2.y);

                if (dist < 100) {
                    ctx.strokeStyle = `rgba(0, 234, 255, ${1 - dist / 100})`;
                    ctx.lineWidth = 0.5;
                    ctx.beginPath();
                    ctx.moveTo(p.x, p.y);
                    ctx.lineTo(p2.x, p2.y);
                    ctx.stroke();
                }
            }
        });
        requestAnimationFrame(animate);
    }

    // Ejecutar cuando el DOM esté listo
    document.addEventListener('DOMContentLoaded', init);
})();
