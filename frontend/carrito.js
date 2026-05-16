let idCliente = 1; // Cambia esto si tienes otro cliente

async function cargarCarrito() {
    const container = document.getElementById('contenidoCarrito');
    const totalEl = document.getElementById('totalCarrito');

    try {
        const res = await fetch(`http://localhost:8080/api/carrito?idCliente=${idCliente}`);
        const items = await res.json();

        container.innerHTML = '';

        let total = 0;

        items.forEach(item => {
            const subtotal = item.cantidad * item.precio;
            total += subtotal;

            container.innerHTML += `
                <div class="col-12 mb-3">
                    <div class="card">
                        <div class="card-body d-flex justify-content-between align-items-center">
                            <div>
                                <strong>${item.nombre}</strong> (${item.referencia})
                                <p class="mb-0">Cantidad: ${item.cantidad} × Q${item.precio}</p>
                            </div>
                            <div class="text-end">
                                <h5>Q ${subtotal.toFixed(2)}</h5>
                            </div>
                        </div>
                    </div>
                </div>`;
        });

        totalEl.textContent = total.toFixed(2);

        if (items.length === 0) {
            container.innerHTML = `<div class="alert alert-info">El carrito está vacío</div>`;
        }
    } catch (e) {
        container.innerHTML = `<div class="alert alert-danger">Error cargando carrito</div>`;
    }
}

async function efectuarCompra() {
    if (!confirm("¿Estás seguro de realizar la compra?")) return;

    try {
        const res = await fetch('http://localhost:8080/api/compra', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                idCliente: idCliente,
                formaPago: "Tarjeta"
            })
        });

        const data = await res.json();

        if (data.exito) {
            alert("¡Compra realizada con éxito! 🎉");
            window.location.href = "dashboard.html";
        } else {
            alert(data.mensaje || "Error al procesar la compra");
        }
    } catch (e) {
        alert("Error de conexión con el servidor");
    }
}

// Cargar automáticamente al abrir la página
window.onload = cargarCarrito;