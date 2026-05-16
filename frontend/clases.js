// =============================================
// CLASES JAVASCRIPT - Programación Orientada a Objetos
// Muebles de los Alpes
// =============================================

const API_BASE = 'http://localhost:8080';

// =============================================
class ApiService {
    constructor(base = API_BASE) {
        this.base = base;
    }
    async get(endpoint, params = {}) {
        const qs = new URLSearchParams(params).toString();
        const url = `${this.base}${endpoint}${qs ? '?' + qs : ''}`;
        const res = await fetch(url);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return res.json();
    }
    async post(endpoint, data) {
        const res = await fetch(`${this.base}${endpoint}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        return res.json();
    }
}

// =============================================
class Sesion {
    static get usuario()    { return localStorage.getItem('usuarioLogueado'); }
    static get idCliente()  { return parseInt(localStorage.getItem('idCliente') || '0'); }
    static get idRol()      { return parseInt(localStorage.getItem('idRol') || '0'); }
    static get nombreRol()  { return localStorage.getItem('nombreRol') || ''; }
    static get esAdmin()    { return Sesion.nombreRol === 'ADMIN'; }
    static get estaLogueado() { return !!Sesion.usuario; }

    static guardar({ username, idCliente, idRol, nombreRol }) {
        localStorage.setItem('usuarioLogueado', username);
        localStorage.setItem('idCliente', idCliente || 0);
        localStorage.setItem('idRol', idRol || 0);
        localStorage.setItem('nombreRol', nombreRol || '');
    }
    static cerrar() { localStorage.clear(); }
    static requiereLogin(redirigir = 'login.html') {
        if (!Sesion.estaLogueado) { window.location.href = redirigir; return false; }
        return true;
    }
    static requiereAdmin(redirigir = 'catalogo.html') {
        if (!Sesion.esAdmin) { window.location.href = redirigir; return false; }
        return true;
    }
}

// =============================================
class Mueble {
    constructor(data) {
        this.idMueble       = data.ID_Mueble      ?? data.ID_MUEBLE      ?? data.idMueble;
        this.referencia     = data.Referencia      ?? data.REFERENCIA     ?? data.referencia;
        this.nombre         = data.Nombre          ?? data.NOMBRE         ?? data.nombre;
        this.descripcion    = data.Descripcion     ?? data.DESCRIPCION    ?? data.descripcion ?? '';
        this.tipo           = data.Tipo            ?? data.TIPO           ?? data.tipo;
        this.categoria      = data.Categoria       ?? data.CATEGORIA      ?? data.categoria ?? '';
        this.idCategoria    = data.ID_Categoria    ?? data.ID_CATEGORIA   ?? data.idCategoria ?? 0;
        this.material       = data.Material        ?? data.MATERIAL       ?? data.material ?? '';
        this.color          = data.Color           ?? data.COLOR          ?? data.color ?? '';
        this.altoCm         = parseFloat(data.AltoCm ?? data.ALTO_CM ?? data.altoCm ?? 0);
        this.anchoCm        = parseFloat(data.AnchoCm ?? data.ANCHO_CM ?? data.anchoCm ?? 0);
        this.profundidadCm  = parseFloat(data.ProfundidadCm ?? data.PROFUNDIDAD_CM ?? data.profundidadCm ?? 0);
        this.pesoGramos     = parseFloat(data.PesoGramos ?? data.PESO_GRAMOS ?? data.pesoGramos ?? 0);
        this.fotoUrl        = data.FotoUrl         ?? data.FOTO_URL       ?? data.fotoUrl ?? '';
        this.precio         = parseFloat(data.Precio ?? data.PRECIO_VENTA ?? data.precio ?? 0);
        this.stock          = parseInt(data.Stock  ?? data.STOCK_DISPONIBLE ?? data.stock ?? 0);
    }
    get precioFormateado() { return `Q ${this.precio.toFixed(2)}`; }
    get tipoBadge() {
        return this.tipo === 'INTERIOR'
            ? '<span class="badge bg-info">Interior</span>'
            : '<span class="badge bg-success">Exterior</span>';
    }
    get imgHtml() {
        return this.fotoUrl
            ? `<img src="${this.fotoUrl}" style="height:160px;object-fit:cover;width:100%" class="rounded-top" onerror="this.style.display='none'">`
            : `<div style="height:160px;background:#eee;display:flex;align-items:center;justify-content:center;color:#aaa;font-size:2.5rem;border-radius:8px 8px 0 0"><i class="fas fa-couch"></i></div>`;
    }
    renderCard(onAgregar) {
        const div = document.createElement('div');
        div.className = 'col-sm-6 col-md-4 col-lg-3';
        div.innerHTML = `
            <div class="card h-100 shadow-sm border-0 mueble-card">
                ${this.imgHtml}
                <div class="card-body d-flex flex-column p-3">
                    ${this.tipoBadge}
                    <h6 class="fw-bold mt-2 mb-1">${this.nombre}</h6>
                    <small class="text-muted">${this.referencia}</small>
                    ${this.material ? `<small class="text-muted"><i class="fas fa-cube me-1"></i>${this.material}${this.color ? ' · ' + this.color : ''}</small>` : ''}
                    <div class="mt-auto pt-2">
                        <div class="fw-bold text-success fs-5">${this.precioFormateado}</div>
                        <small class="text-muted">Stock: ${this.stock}</small>
                        <button class="btn btn-success btn-sm w-100 mt-2 btn-agregar">
                            <i class="fas fa-cart-plus me-1"></i>Agregar
                        </button>
                    </div>
                </div>
            </div>`;
        div.querySelector('.btn-agregar').onclick = () => onAgregar(this);
        return div;
    }
}

// =============================================
class Cliente {
    constructor(data) {
        this.idCliente    = data.ID_CLIENTE    ?? data.idCliente;
        this.numDoc       = data.NUM_DOC       ?? data.numDoc;
        this.nombre       = data.NOMBRE_COMPLETO ?? data.nombre;
        this.email        = data.EMAIL         ?? data.email;
        this.tel          = data.TEL_RESIDENCIA ?? data.tel;
        this.tipoPersona  = data.TIPO_PERSONA  ?? data.tipoPersona;
        this.ciudad       = data.CIUDAD        ?? data.ciudad;
        this.estado       = data.ESTADO        ?? data.estado;
        this.totalCompras = parseInt(data.TOTAL_COMPRAS ?? data.totalCompras ?? 0);
    }
    renderFila(onEliminar) {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${this.numDoc}</td>
            <td>${this.nombre}</td>
            <td>${this.email}</td>
            <td>${this.ciudad ?? '-'}</td>
            <td><span class="badge ${this.totalCompras > 0 ? 'bg-success' : 'bg-secondary'}">${this.totalCompras}</span></td>
            <td>
                <button class="btn btn-danger btn-sm btn-eliminar" ${this.totalCompras > 0 ? 'disabled title="Tiene compras"' : ''}>
                    <i class="fas fa-trash"></i>
                </button>
            </td>`;
        if (this.totalCompras === 0)
            tr.querySelector('.btn-eliminar').onclick = () => onEliminar(this);
        return tr;
    }
}

// =============================================
class CarritoItem {
    constructor(data) {
        this.idDetalle  = data.idDetalle  ?? data.ID_DETALLE;
        this.nombre     = data.nombre     ?? data.NOMBRE;
        this.referencia = data.referencia ?? data.REFERENCIA;
        this.cantidad   = parseInt(data.cantidad ?? data.CANTIDAD);
        this.precio     = parseFloat(data.precio ?? data.PRECIO);
        this.subtotal   = parseFloat(data.subtotal ?? data.SUBTOTAL);
    }
    get subtotalFormateado() { return `Q ${this.subtotal.toFixed(2)}`; }
}

// =============================================
class MuebleService extends ApiService {
    async obtenerTodos()               { return (await this.get('/api/muebles')).map(d => new Mueble(d)); }
    async buscarAdmin(criterio, tipo)  { return this.get('/api/admin/muebles', { criterio, tipo }); }
    async detalle(id)                  { return this.get('/api/muebles/detalle', { id }); }
    async categorias()                 { return this.get('/api/admin/categorias'); }
    async modificar(data)              { return this.post('/api/admin/muebles/modificar', data); }
    async precioStock(data)            { return this.post('/api/admin/muebles/precio-stock', data); }
    async eliminar(idMueble)           { return this.post('/api/admin/muebles/eliminar', { idMueble }); }
    async agregar(data)                { return this.post('/api/carrito/agregar', data); }
}

// =============================================
class ClienteService extends ApiService {
    async buscar(criterio)     { return this.get('/api/admin/clientes', { criterio }); }
    async eliminar(idCliente)  { return this.post('/api/admin/clientes/eliminar', { idCliente }); }
    async perfil(idCliente)    { return this.get('/api/clientes/perfil', { idCliente }); }
}

// =============================================
class ReporteService extends ApiService {
    async ventasDiarias(fechaIni, fechaFin, idCiudad = 0) {
        return this.get('/api/reportes/ventas-diarias', { fechaIni, fechaFin, idCiudad });
    }
    async productoTop(fechaIni, fechaFin, idCiudad = 0) {
        return this.get('/api/reportes/producto-top', { fechaIni, fechaFin, idCiudad });
    }
    async historialCliente(idCliente) {
        return this.get('/api/reportes/historial-cliente', { idCliente });
    }
    async cierreCajas(fechaIni, fechaFin) {
        return this.get('/api/reportes/cierre-cajas', { fechaIni, fechaFin });
    }
    async ltv()          { return this.get('/api/reportes/ltv'); }
    async actividad(fechaIni, fechaFin) {
        return this.get('/api/reportes/actividad', { fechaIni, fechaFin });
    }
    async retencion()    { return this.get('/api/reportes/retencion'); }
    async cohorte()      { return this.get('/api/reportes/cohorte'); }
    async remarketing()  { return this.get('/api/reportes/remarketing'); }
}

// Instancias globales
const api = new ApiService();
const muebleService = new MuebleService();
const clienteService = new ClienteService();
const reporteService = new ReporteService();
