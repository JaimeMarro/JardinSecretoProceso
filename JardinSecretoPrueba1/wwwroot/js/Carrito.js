document.addEventListener("DOMContentLoaded", () => {
    const STORAGE_KEY = "carrito_v1";
    const tbody = document.querySelector("#tabla-carrito tbody");
    const totalEl = document.getElementById("total");
    const btnVaciar = document.getElementById("vaciar");
    const btnEnviar = document.getElementById("enviar");

    // Evento: Agregar producto desde botón
    document.querySelectorAll(".add-to-cart").forEach(btn => {
        btn.addEventListener("click", () => {
            const id = btn.dataset.id;
            const nombre = btn.dataset.nombre;
            const precio = parseFloat(btn.dataset.precio);

            let carrito = leerCarrito();

            // Verificar si ya existe
            let existente = carrito.find(p => p.id == id);
            if (existente) {
                existente.cantidad += 1;
            } else {
                carrito.push({
                    id: id,
                    nombre: nombre,
                    precio: precio,
                    cantidad: 1
                });
            }

            guardarCarrito(carrito);

            alert(`${nombre} agregado al carrito`);
        });
    });
    function leerCarrito() {
        try {
            const raw = localStorage.getItem(STORAGE_KEY);
            return raw ? JSON.parse(raw) : [];
        } catch (e) {
            console.error("Error al leer carrito:", e);
            return [];
        }
    }

    function guardarCarrito(arr) {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(arr));
        renderizar();
    }

    function renderizar() {
        const carrito = leerCarrito();
        tbody.innerHTML = "";
        let total = 0;

        if (carrito.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center">Tu carrito está vacío.</td></tr>`;
            totalEl.textContent = "0.00";
            return;
        }

        carrito.forEach((item, i) => {
            const precio = Number(item.precio) || 0;
            const cantidad = Number(item.cantidad) || 0;
            const subtotal = precio * cantidad;
            total += subtotal;

            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${escapeHtml(item.nombre)}</td>
                <td>$${precio.toFixed(2)}</td>
                <td>
                    <button class="menos btn btn-sm btn-secondary" data-index="${i}">-</button>
                    ${cantidad}
                    <button class="mas btn btn-sm btn-secondary" data-index="${i}">+</button>
                </td>
                <td>$${subtotal.toFixed(2)}</td>
                <td><button class="eliminar btn btn-sm btn-danger" data-index="${i}">Eliminar</button></td>
            `;
            tbody.appendChild(tr);
        });

        totalEl.textContent = total.toFixed(2);
    }

    function escapeHtml(text) {
        if (!text) return "";
        return text.replace(/[&<>"'`=\/]/g, function (s) {
            return "&#" + s.charCodeAt(0) + ";";
        });
    }

    // Acciones de los botones dentro de la tabla
    tbody.addEventListener("click", (e) => {
        let carrito = leerCarrito();
        if (e.target.matches(".mas")) {
            const i = Number(e.target.dataset.index);
            carrito[i].cantidad = Number(carrito[i].cantidad) + 1;
            guardarCarrito(carrito);
            return;
        }
        if (e.target.matches(".menos")) {
            const i = Number(e.target.dataset.index);
            carrito[i].cantidad = Number(carrito[i].cantidad) - 1;
            if (carrito[i].cantidad <= 0) carrito.splice(i, 1);
            guardarCarrito(carrito);
            return;
        }
        if (e.target.matches(".eliminar")) {
            const i = Number(e.target.dataset.index);
            carrito.splice(i, 1);
            guardarCarrito(carrito);
            return;
        }
    });

    // Vaciar carrito
    btnVaciar.addEventListener("click", () => {
        if (!confirm("¿Vaciar todo el carrito?")) return;
        localStorage.removeItem(STORAGE_KEY);
        renderizar();
    });

    // Enviar por WhatsApp
    btnEnviar.addEventListener("click", () => {
        const carrito = leerCarrito();
        if (carrito.length === 0) {
            alert("El carrito está vacío");
            return;
        }
        let mensaje = "Hola, quiero este pedido:%0A";
        carrito.forEach(it => {
            mensaje += `- ${it.nombre} x${it.cantidad}: $${(it.precio * it.cantidad).toFixed(2)}%0A`;
        });
        const total = carrito.reduce((s, it) => s + it.precio * it.cantidad, 0);
        mensaje += `Total: $${total.toFixed(2)}`;

        const telefono = "70857606"; // tu número de WhatsApp
        window.open(`https://wa.me/${telefono}?text=${mensaje}`, "_blank");
    });

    // Render inicial
    renderizar();
});
