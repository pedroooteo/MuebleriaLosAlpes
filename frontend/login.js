document.getElementById('formLogin').addEventListener('submit', async function(e) {
    e.preventDefault();

    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value.trim();
    const alerta = document.getElementById('alertaMensaje');
    const btn = document.querySelector('button[type="submit"]');

    btn.disabled = true;
    btn.textContent = 'Verificando...';

    try {
        const respuesta = await fetch('https://tinsmith-unlocked-squealing.ngrok-free.dev/api/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'ngrok-skip-browser-warning': '1' },
            body: JSON.stringify({ Username: username, Password: password })
        });

        const resultado = await respuesta.json();
        alerta.classList.remove('d-none', 'alert-success', 'alert-danger');

        if (resultado.exito) {
            alerta.classList.add('alert-success');
            alerta.textContent = '¡Bienvenido ' + username + '! Redirigiendo...';

            localStorage.setItem('usuarioLogueado', resultado.username || username);
            localStorage.setItem('idCliente', resultado.idCliente || '0');
            localStorage.setItem('idRol', resultado.idRol || '0');
            localStorage.setItem('nombreRol', resultado.nombreRol || '');

            const destino = (resultado.nombreRol === 'ADMIN') ? 'admin.html' : 'catalogo.html';
            setTimeout(() => { window.location.href = destino; }, 1000);
        } else {
            alerta.classList.add('alert-danger');
            alerta.textContent = resultado.mensaje || 'Usuario o contraseña incorrectos';
            btn.disabled = false;
            btn.textContent = 'Iniciar Sesión';
        }
    } catch (error) {
        console.error(error);
        alerta.classList.remove('d-none');
        alerta.classList.add('alert-danger');
        alerta.textContent = "No se pudo conectar con el servidor. ¿Está corriendo el backend?";
        btn.disabled = false;
        btn.textContent = 'Iniciar Sesión';
    }
});