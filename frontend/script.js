let idClienteActual = parseInt(localStorage.getItem('idCliente') || '1');

// ==================== DASHBOARD ====================
async function cargarMuebles() {
    const container = document.getElementById('listaMuebles');
    if (!container) return;

    console.log("🔄 Intentando cargar muebles...");
    try {
        const res = await fetch('http://localhost:8080/api/muebles');
        console.log("📡 Status del servidor:", res.status);
        
        const muebles = await res.json();
        console.log("📦 Respuesta cruda:", muebles);
        console.log("✅ Muebles recibidos:", muebles);

        container.innerHTML = '';

        muebles.forEach(m => {
            const card = document.createElement('div');
            card.className = 'col-md-4 mb-4';
            card.innerHTML = `
                <div class="card h-100 mueble-card">
                    <div class="card-body">
                        <span class="badge bg-info">${m.Tipo}</span>
                        <h5 class="card-title mt-2">${m.Nombre}</h5>
                        <p class="text-muted">${m.Referencia}</p>
                        <p class="fs-4 fw-bold text-success">Q ${parseFloat(m.Precio).toFixed(2)}</p>
                        <p class="text-success small">Stock: ${m.Stock}</p>
                        <button onclick="agregarAlCarrito(${m.ID_Mueble})" 
                                class="btn btn-success w-100">
                            🛒 Agregar al carrito
                        </button>
                    </div>
                </div>`;
            container.appendChild(card);
        });
    } catch (e) {
        console.error("❌ Error cargando muebles:", e);
    }
}

// ==================== AGREGAR AL CARRITO (con debug) ====================
async function agregarAlCarrito(idMueble) {
    console.log(`🛒 Intentando agregar mueble ID: ${idMueble}`);

    try {
        const response = await fetch('http://localhost:8080/api/carrito/agregar', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                idCliente: idClienteActual,
                idMueble: idMueble,
                cantidad: 1
            })
        });

        console.log("📡 Status de agregar:", response.status);

        if (response.ok) {
            const data = await response.json();
            console.log("✅ Respuesta del servidor:", data);
            alert('✅ Producto agregado al carrito!');
        } else {
            console.error("❌ Error del servidor:", response.status);
            alert('Error al agregar al carrito');
        }
    } catch (e) {
        console.error("❌ Error en fetch:", e);
        alert('No se pudo conectar con el servidor');
    }
}

// ==================== CARRITO VISUAL ====================
async function cargarCarrito() {
    const container = document.getElementById('contenidoCarrito');
    if (!container) return;

    try {
        const res = await fetch(`http://localhost:8080/api/carrito?idCliente=${idClienteActual}`);
        const items = await res.json();

        let total = 0;
        container.innerHTML = '';

        if (items.length === 0) {
            container.innerHTML = `<div class="col-12 text-center py-5"><h4>🛒 El carrito está vacío</h4></div>`;
            return;
        }

        items.forEach(item => {
            const subtotal = parseFloat(item.subtotal || item.Subtotal);
            total += subtotal;
            const div = document.createElement('div');
            div.className = 'col-12 mb-3';
            div.innerHTML = `
                <div class="card">
                    <div class="card-body d-flex justify-content-between">
                        <div>
                            <strong>${item.nombre || item.Nombre}</strong><br>
                            <small>${item.referencia || item.Referencia} × ${item.cantidad || item.Cantidad}</small>
                        </div>
                        <div class="text-end">
                            <h5>Q ${subtotal.toFixed(2)}</h5>
                        </div>
                    </div>
                </div>`;
            container.appendChild(div);
        });

        document.getElementById('totalCarrito').textContent = `Q ${total.toFixed(2)}`;
    } catch (e) {
        console.error(e);
    }
}

async function efectuarCompra() {
    if (!confirm('¿Confirmar compra?')) return;
    try {
        const res = await fetch('http://localhost:8080/api/compra', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ idCliente: idClienteActual, formaPago: 'Tarjeta' })
        });
        const data = await res.json();
        if (data.exito) {
            alert('🎉 ¡Compra realizada con éxito!');
            window.location.href = 'dashboard.html';
        } else {
            alert(data.mensaje);
        }
    } catch (e) {
        alert('Error en la compra');
    }
}

// ==================== INICIALIZACIÓN ====================
if (window.location.pathname.includes('dashboard.html')) {
    document.getElementById('usuarioActual').textContent = 'Usuario: ' + (localStorage.getItem('usuarioLogueado') || 'Teo');
    cargarMuebles();
}

if (window.location.pathname.includes('carrito.html')) {
    cargarCarrito();
}