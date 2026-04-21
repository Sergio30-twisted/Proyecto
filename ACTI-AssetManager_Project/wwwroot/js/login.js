/* ---------------------------------------------------
    1. ANIMACIÓN DE FONDO (NODOS)
--------------------------------------------------- */
const canvas = document.getElementById("nodes");
const ctx = canvas.getContext("2d");

function resizeCanvas() {
    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;
}

resizeCanvas();

let particles = [];
const numParticles = 80;

class Particle {
    constructor() {
        this.x = Math.random() * canvas.width;
        this.y = Math.random() * canvas.height;
        this.vx = (Math.random() - 0.5) * 0.7;
        this.vy = (Math.random() - 0.5) * 0.7;
        this.radius = 2;
    }

    move() {
        this.x += this.vx;
        this.y += this.vy;
        if (this.x < 0 || this.x > canvas.width) this.vx *= -1;
        if (this.y < 0 || this.y > canvas.height) this.vy *= -1;
    }

    draw() {
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fillStyle = "rgba(120, 180, 255, 0.8)";
        ctx.fill();
    }
}

function connectParticles() {
    for (let a = 0; a < particles.length; a++) {
        for (let b = a; b < particles.length; b++) {
            let dx = particles[a].x - particles[b].x;
            let dy = particles[a].y - particles[b].y;
            let dist = Math.sqrt(dx * dx + dy * dy);
            if (dist < 120) {
                ctx.beginPath();
                ctx.strokeStyle = `rgba(120,180,255,${1 - dist / 120})`;
                ctx.lineWidth = 0.5;
                ctx.moveTo(particles[a].x, particles[a].y);
                ctx.lineTo(particles[b].x, particles[b].y);
                ctx.stroke();
            }
        }
    }
}

function animate() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    particles.forEach(p => {
        p.move();
        p.draw();
    });
    connectParticles();
    requestAnimationFrame(animate);
}

function initParticles() {
    particles = [];
    for (let i = 0; i < numParticles; i++) {
        particles.push(new Particle());
    }
}

initParticles();
animate();

window.addEventListener("resize", () => {
    resizeCanvas();
    initParticles();
});

/* ---------------------------------------------------
    2. LÓGICA DE LOGIN (Punto de conexión)
--------------------------------------------------- */
const loginForm = document.getElementById("loginForm");

if (loginForm) {
    loginForm.addEventListener("submit", async (e) => {
        e.preventDefault();

        // Extraemos los datos del formulario (Username/Email y Password)
        const formData = new FormData(loginForm);
        const loginData = Object.fromEntries(formData.entries());

        try {
            // Enviamos los datos al servidor (Ajusta la URL a tu API/Controlador)
            const response = await fetch('/Auth/Login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(loginData)
            });

            if (response.ok) {
                const data = await response.json();

                // GUARDAMOS EL TOKEN (Esto quita el "Invitado")
                if (data.token) {
                    localStorage.setItem("token", data.token);
                    console.log("Token guardado con éxito.");

                    // Redirigimos al Home
                    window.location.href = "/Home/Index";
                } else {
                    alert("Error: El servidor no envió un token válido.");
                }
            } else {
                alert("Usuario o contraseña incorrectos");
            }
        } catch (error) {
            console.error("Error en la petición de login:", error);
            alert("Hubo un error al conectar con el servidor.");
        }
    });
}