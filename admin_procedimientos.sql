-- =============================================
-- PROCEDIMIENTOS ADMINISTRACIÓN - MUEBLES DE LOS ALPES
-- Ejecutar completo en Oracle SQL Developer
-- =============================================

-- =============================================
-- PKG_MUEBLES (reemplaza el paquete completo)
-- =============================================
CREATE OR REPLACE PACKAGE PKG_MUEBLES AS
    PROCEDURE PR_CONSULTAR_MUEBLES(
        p_criterio IN VARCHAR2 DEFAULT NULL,
        p_tipo     IN VARCHAR2 DEFAULT NULL,
        p_cursor   OUT SYS_REFCURSOR
    );
    PROCEDURE PR_BUSCAR_MUEBLES_ADMIN(
        p_criterio IN VARCHAR2 DEFAULT NULL,
        p_tipo     IN VARCHAR2 DEFAULT NULL,
        p_cursor   OUT SYS_REFCURSOR
    );
    PROCEDURE PR_CREAR_MUEBLE(
        p_referencia IN VARCHAR2, p_nombre IN VARCHAR2, p_descripcion IN CLOB,
        p_tipo IN VARCHAR2, p_id_categoria IN NUMBER, p_material IN VARCHAR2,
        p_alto_cm IN NUMBER, p_ancho_cm IN NUMBER, p_profundidad_cm IN NUMBER,
        p_color IN VARCHAR2, p_peso_gramos IN NUMBER, p_foto_url IN VARCHAR2,
        p_precio IN NUMBER, p_stock IN NUMBER
    );
    PROCEDURE PR_OBTENER_MUEBLE(p_id_mueble IN NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE PR_MODIFICAR_MUEBLE(
        p_id_mueble IN NUMBER, p_referencia IN VARCHAR2, p_nombre IN VARCHAR2,
        p_descripcion IN CLOB, p_tipo IN VARCHAR2, p_id_categoria IN NUMBER,
        p_material IN VARCHAR2, p_alto_cm IN NUMBER, p_ancho_cm IN NUMBER,
        p_profundidad_cm IN NUMBER, p_color IN VARCHAR2, p_peso_gramos IN NUMBER,
        p_foto_url IN VARCHAR2
    );
    PROCEDURE PR_ACTUALIZAR_PRECIO_STOCK(
        p_id_mueble IN NUMBER, p_precio IN NUMBER, p_stock IN NUMBER
    );
    PROCEDURE PR_ELIMINAR_MUEBLE(p_id_mueble IN NUMBER);
    PROCEDURE PR_LISTAR_CATEGORIAS(p_cursor OUT SYS_REFCURSOR);
END PKG_MUEBLES;
/

CREATE OR REPLACE PACKAGE BODY PKG_MUEBLES AS

    PROCEDURE PR_CONSULTAR_MUEBLES(
        p_criterio IN VARCHAR2 DEFAULT NULL,
        p_tipo     IN VARCHAR2 DEFAULT NULL,
        p_cursor   OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT ID_MUEBLE, REFERENCIA, NOMBRE, TIPO, PRECIO_VENTA, STOCK_DISPONIBLE,
               FOTO_URL, MATERIAL, COLOR, DESCRIPCION
        FROM MUEBLES
        WHERE (p_criterio IS NULL
               OR UPPER(NOMBRE) LIKE '%'||UPPER(p_criterio)||'%'
               OR UPPER(REFERENCIA) LIKE '%'||UPPER(p_criterio)||'%')
          AND (p_tipo IS NULL OR TIPO = p_tipo)
          AND STOCK_DISPONIBLE > 0;
    END PR_CONSULTAR_MUEBLES;

    PROCEDURE PR_BUSCAR_MUEBLES_ADMIN(
        p_criterio IN VARCHAR2 DEFAULT NULL,
        p_tipo     IN VARCHAR2 DEFAULT NULL,
        p_cursor   OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT M.ID_MUEBLE, M.REFERENCIA, M.NOMBRE, M.TIPO, M.PRECIO_VENTA,
               M.STOCK_DISPONIBLE, M.FOTO_URL, M.MATERIAL, M.COLOR,
               M.ALTO_CM, M.ANCHO_CM, M.PROFUNDIDAD_CM, M.PESO_GRAMOS,
               C.NOMBRE AS CATEGORIA, M.DESCRIPCION, M.ID_CATEGORIA
        FROM MUEBLES M
        LEFT JOIN CATEGORIAS C ON M.ID_CATEGORIA = C.ID_CATEGORIA
        WHERE (p_criterio IS NULL
               OR UPPER(M.NOMBRE) LIKE '%'||UPPER(p_criterio)||'%'
               OR UPPER(M.REFERENCIA) LIKE '%'||UPPER(p_criterio)||'%')
          AND (p_tipo IS NULL OR M.TIPO = p_tipo)
        ORDER BY M.FECHA_CREACION DESC;
    END PR_BUSCAR_MUEBLES_ADMIN;

    PROCEDURE PR_CREAR_MUEBLE(
        p_referencia IN VARCHAR2, p_nombre IN VARCHAR2, p_descripcion IN CLOB,
        p_tipo IN VARCHAR2, p_id_categoria IN NUMBER, p_material IN VARCHAR2,
        p_alto_cm IN NUMBER, p_ancho_cm IN NUMBER, p_profundidad_cm IN NUMBER,
        p_color IN VARCHAR2, p_peso_gramos IN NUMBER, p_foto_url IN VARCHAR2,
        p_precio IN NUMBER, p_stock IN NUMBER
    ) IS
        v_id_mueble NUMBER;
    BEGIN
        INSERT INTO MUEBLES (REFERENCIA, NOMBRE, DESCRIPCION, TIPO, ID_CATEGORIA, MATERIAL,
            ALTO_CM, ANCHO_CM, PROFUNDIDAD_CM, COLOR, PESO_GRAMOS, FOTO_URL,
            PRECIO_VENTA, STOCK_DISPONIBLE)
        VALUES (p_referencia, p_nombre, p_descripcion, p_tipo, p_id_categoria, p_material,
            p_alto_cm, p_ancho_cm, p_profundidad_cm, p_color, p_peso_gramos, p_foto_url,
            p_precio, p_stock)
        RETURNING ID_MUEBLE INTO v_id_mueble;

        INSERT INTO HISTORIAL_PRECIOS (ID_MUEBLE, PRECIO)
        VALUES (v_id_mueble, p_precio);

        COMMIT;
    EXCEPTION WHEN OTHERS THEN ROLLBACK; RAISE;
    END PR_CREAR_MUEBLE;

    PROCEDURE PR_OBTENER_MUEBLE(p_id_mueble IN NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT M.ID_MUEBLE, M.REFERENCIA, M.NOMBRE, M.DESCRIPCION, M.TIPO,
               M.MATERIAL, M.ALTO_CM, M.ANCHO_CM, M.PROFUNDIDAD_CM,
               M.COLOR, M.PESO_GRAMOS, M.FOTO_URL, M.PRECIO_VENTA, M.STOCK_DISPONIBLE,
               C.NOMBRE AS CATEGORIA, M.ID_CATEGORIA
        FROM MUEBLES M
        LEFT JOIN CATEGORIAS C ON M.ID_CATEGORIA = C.ID_CATEGORIA
        WHERE M.ID_MUEBLE = p_id_mueble;
    END PR_OBTENER_MUEBLE;

    PROCEDURE PR_MODIFICAR_MUEBLE(
        p_id_mueble IN NUMBER, p_referencia IN VARCHAR2, p_nombre IN VARCHAR2,
        p_descripcion IN CLOB, p_tipo IN VARCHAR2, p_id_categoria IN NUMBER,
        p_material IN VARCHAR2, p_alto_cm IN NUMBER, p_ancho_cm IN NUMBER,
        p_profundidad_cm IN NUMBER, p_color IN VARCHAR2, p_peso_gramos IN NUMBER,
        p_foto_url IN VARCHAR2
    ) IS
    BEGIN
        UPDATE MUEBLES SET
            REFERENCIA = p_referencia, NOMBRE = p_nombre, DESCRIPCION = p_descripcion,
            TIPO = p_tipo, ID_CATEGORIA = p_id_categoria, MATERIAL = p_material,
            ALTO_CM = p_alto_cm, ANCHO_CM = p_ancho_cm, PROFUNDIDAD_CM = p_profundidad_cm,
            COLOR = p_color, PESO_GRAMOS = p_peso_gramos, FOTO_URL = p_foto_url
        WHERE ID_MUEBLE = p_id_mueble;
        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20010, 'Mueble no encontrado');
        END IF;
        COMMIT;
    EXCEPTION WHEN OTHERS THEN ROLLBACK; RAISE;
    END PR_MODIFICAR_MUEBLE;

    PROCEDURE PR_ACTUALIZAR_PRECIO_STOCK(
        p_id_mueble IN NUMBER, p_precio IN NUMBER, p_stock IN NUMBER
    ) IS
        v_precio_anterior NUMBER;
    BEGIN
        SELECT PRECIO_VENTA INTO v_precio_anterior
        FROM MUEBLES WHERE ID_MUEBLE = p_id_mueble;

        UPDATE MUEBLES
        SET PRECIO_VENTA = p_precio, STOCK_DISPONIBLE = p_stock
        WHERE ID_MUEBLE = p_id_mueble;

        IF v_precio_anterior <> p_precio THEN
            INSERT INTO HISTORIAL_PRECIOS (ID_MUEBLE, PRECIO)
            VALUES (p_id_mueble, p_precio);
        END IF;
        COMMIT;
    EXCEPTION WHEN OTHERS THEN ROLLBACK; RAISE;
    END PR_ACTUALIZAR_PRECIO_STOCK;

    PROCEDURE PR_ELIMINAR_MUEBLE(p_id_mueble IN NUMBER) IS
        v_compras NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_compras
        FROM DETALLE_COMPRA WHERE ID_MUEBLE = p_id_mueble;

        IF v_compras > 0 THEN
            RAISE_APPLICATION_ERROR(-20011,
                'No se puede eliminar: el mueble ha sido comprado por ' || v_compras || ' cliente(s)');
        END IF;

        DELETE FROM DETALLE_CARRITO WHERE ID_MUEBLE = p_id_mueble;
        DELETE FROM HISTORIAL_PRECIOS WHERE ID_MUEBLE = p_id_mueble;
        DELETE FROM INVENTARIO_MOVIMIENTOS WHERE ID_MUEBLE = p_id_mueble;
        DELETE FROM MUEBLES WHERE ID_MUEBLE = p_id_mueble;
        COMMIT;
    EXCEPTION WHEN OTHERS THEN ROLLBACK; RAISE;
    END PR_ELIMINAR_MUEBLE;

    PROCEDURE PR_LISTAR_CATEGORIAS(p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT ID_CATEGORIA, NOMBRE FROM CATEGORIAS ORDER BY NOMBRE;
    END PR_LISTAR_CATEGORIAS;

END PKG_MUEBLES;
/

-- =============================================
-- PKG_CLIENTES (reemplaza el paquete completo)
-- =============================================
CREATE OR REPLACE PACKAGE PKG_CLIENTES AS
    PROCEDURE PR_LISTAR_CIUDADES(p_cursor OUT SYS_REFCURSOR);
    PROCEDURE PR_OBTENER_PERFIL(p_id_cliente IN NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE PR_BUSCAR_CLIENTES(p_criterio IN VARCHAR2, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE PR_ELIMINAR_CLIENTE(p_id_cliente IN NUMBER);
END PKG_CLIENTES;
/

CREATE OR REPLACE PACKAGE BODY PKG_CLIENTES AS

    PROCEDURE PR_LISTAR_CIUDADES(p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT CI.ID_CIUDAD, CI.NOMBRE AS CIUDAD,
               D.NOMBRE AS DEPARTAMENTO, P.NOMBRE AS PAIS
        FROM CIUDADES CI
        JOIN DEPARTAMENTOS D ON CI.ID_DEPTO = D.ID_DEPTO
        JOIN PAISES P ON D.ID_PAIS = P.ID_PAIS
        ORDER BY P.NOMBRE, D.NOMBRE, CI.NOMBRE;
    END PR_LISTAR_CIUDADES;

    PROCEDURE PR_OBTENER_PERFIL(p_id_cliente IN NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT CL.ID_CLIENTE, CL.NOMBRE_COMPLETO, CL.EMAIL, CL.TEL_RESIDENCIA,
               CL.TEL_CELULAR, CL.DIRECCION, CL.TIPO_DOC, CL.NUM_DOC,
               CL.TIPO_PERSONA, CL.NIT, CL.PROFESION,
               CI.NOMBRE AS CIUDAD, D.NOMBRE AS DEPARTAMENTO
        FROM CLIENTES CL
        LEFT JOIN CIUDADES CI ON CL.ID_CIUDAD = CI.ID_CIUDAD
        LEFT JOIN DEPARTAMENTOS D ON CI.ID_DEPTO = D.ID_DEPTO
        WHERE CL.ID_CLIENTE = p_id_cliente;
    END PR_OBTENER_PERFIL;

    PROCEDURE PR_BUSCAR_CLIENTES(p_criterio IN VARCHAR2, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT CL.ID_CLIENTE, CL.NUM_DOC, CL.NOMBRE_COMPLETO, CL.EMAIL,
               CL.TEL_RESIDENCIA, CL.TIPO_PERSONA, CL.ESTADO,
               CI.NOMBRE AS CIUDAD,
               (SELECT COUNT(*) FROM COMPRAS WHERE ID_CLIENTE = CL.ID_CLIENTE) AS TOTAL_COMPRAS
        FROM CLIENTES CL
        LEFT JOIN CIUDADES CI ON CL.ID_CIUDAD = CI.ID_CIUDAD
        WHERE (p_criterio IS NULL
               OR UPPER(CL.NUM_DOC)        LIKE '%'||UPPER(p_criterio)||'%'
               OR UPPER(CL.NOMBRE_COMPLETO) LIKE '%'||UPPER(p_criterio)||'%'
               OR UPPER(CL.EMAIL)           LIKE '%'||UPPER(p_criterio)||'%')
        ORDER BY CL.NOMBRE_COMPLETO;
    END PR_BUSCAR_CLIENTES;

    PROCEDURE PR_ELIMINAR_CLIENTE(p_id_cliente IN NUMBER) IS
        v_compras NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_compras
        FROM COMPRAS WHERE ID_CLIENTE = p_id_cliente;

        IF v_compras > 0 THEN
            RAISE_APPLICATION_ERROR(-20012,
                'No se puede eliminar: el cliente tiene ' || v_compras || ' compra(s) registrada(s)');
        END IF;

        DELETE FROM DETALLE_CARRITO DC
        WHERE DC.ID_CARRITO IN (
            SELECT ID_CARRITO FROM CARRITO WHERE ID_CLIENTE = p_id_cliente
        );
        DELETE FROM CARRITO WHERE ID_CLIENTE = p_id_cliente;
        DELETE FROM USUARIOS_SISTEMA WHERE ID_CLIENTE = p_id_cliente;
        DELETE FROM CLIENTES WHERE ID_CLIENTE = p_id_cliente;
        COMMIT;
    EXCEPTION WHEN OTHERS THEN ROLLBACK; RAISE;
    END PR_ELIMINAR_CLIENTE;

END PKG_CLIENTES;
/

-- =============================================
-- PKG_REPORTES (nuevo - misma BD como replica)
-- =============================================
CREATE OR REPLACE PACKAGE PKG_REPORTES AS
    PROCEDURE PR_VENTAS_DIARIAS(
        p_fecha_ini IN DATE, p_fecha_fin IN DATE,
        p_id_ciudad IN NUMBER DEFAULT NULL,
        p_cursor OUT SYS_REFCURSOR
    );
    PROCEDURE PR_PRODUCTO_TOP(
        p_fecha_ini IN DATE, p_fecha_fin IN DATE,
        p_id_ciudad IN NUMBER DEFAULT NULL,
        p_cursor OUT SYS_REFCURSOR
    );
    PROCEDURE PR_HISTORIAL_CLIENTE_ADMIN(
        p_id_cliente IN NUMBER, p_cursor OUT SYS_REFCURSOR
    );
    PROCEDURE PR_CIERRE_CAJAS(
        p_fecha_ini IN DATE, p_fecha_fin IN DATE,
        p_cursor OUT SYS_REFCURSOR
    );
    PROCEDURE PR_REPORTE_LTV(p_cursor OUT SYS_REFCURSOR);
    PROCEDURE PR_REPORTE_ACTIVIDAD(
        p_fecha_ini IN DATE, p_fecha_fin IN DATE,
        p_cursor OUT SYS_REFCURSOR
    );
    PROCEDURE PR_REPORTE_RETENCION(p_cursor OUT SYS_REFCURSOR);
    PROCEDURE PR_REPORTE_COHORTE(p_cursor OUT SYS_REFCURSOR);
    PROCEDURE PR_REPORTE_REMARKETING(p_cursor OUT SYS_REFCURSOR);
END PKG_REPORTES;
/

CREATE OR REPLACE PACKAGE BODY PKG_REPORTES AS

    PROCEDURE PR_VENTAS_DIARIAS(
        p_fecha_ini IN DATE, p_fecha_fin IN DATE,
        p_id_ciudad IN NUMBER DEFAULT NULL,
        p_cursor OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT M.TIPO,
               TO_CHAR(C.FECHA_COMPRA, 'DD/MM/YYYY') AS FECHA,
               COUNT(DISTINCT C.ID_COMPRA)  AS TOTAL_ORDENES,
               SUM(DC.CANTIDAD)             AS UNIDADES_VENDIDAS,
               SUM(DC.SUBTOTAL)             AS INGRESOS
        FROM COMPRAS C
        JOIN DETALLE_COMPRA DC ON C.ID_COMPRA = DC.ID_COMPRA
        JOIN MUEBLES M         ON DC.ID_MUEBLE = M.ID_MUEBLE
        JOIN CLIENTES CL       ON C.ID_CLIENTE = CL.ID_CLIENTE
        WHERE TRUNC(C.FECHA_COMPRA) BETWEEN p_fecha_ini AND p_fecha_fin
          AND C.ESTADO = 'PAGADO'
          AND (p_id_ciudad IS NULL OR CL.ID_CIUDAD = p_id_ciudad)
        GROUP BY M.TIPO, TO_CHAR(C.FECHA_COMPRA, 'DD/MM/YYYY')
        ORDER BY 2, 1;
    END PR_VENTAS_DIARIAS;

    PROCEDURE PR_PRODUCTO_TOP(
        p_fecha_ini IN DATE, p_fecha_fin IN DATE,
        p_id_ciudad IN NUMBER DEFAULT NULL,
        p_cursor OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT M.ID_MUEBLE, M.REFERENCIA, M.NOMBRE, M.TIPO,
               SUM(DC.CANTIDAD)          AS UNIDADES_VENDIDAS,
               SUM(DC.SUBTOTAL)          AS INGRESOS_TOTALES,
               COUNT(DISTINCT C.ID_COMPRA) AS ORDENES
        FROM COMPRAS C
        JOIN DETALLE_COMPRA DC ON C.ID_COMPRA = DC.ID_COMPRA
        JOIN MUEBLES M         ON DC.ID_MUEBLE = M.ID_MUEBLE
        JOIN CLIENTES CL       ON C.ID_CLIENTE = CL.ID_CLIENTE
        WHERE TRUNC(C.FECHA_COMPRA) BETWEEN p_fecha_ini AND p_fecha_fin
          AND C.ESTADO = 'PAGADO'
          AND (p_id_ciudad IS NULL OR CL.ID_CIUDAD = p_id_ciudad)
        GROUP BY M.ID_MUEBLE, M.REFERENCIA, M.NOMBRE, M.TIPO
        ORDER BY UNIDADES_VENDIDAS DESC;
    END PR_PRODUCTO_TOP;

    PROCEDURE PR_HISTORIAL_CLIENTE_ADMIN(
        p_id_cliente IN NUMBER, p_cursor OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT C.NUMERO_ORDEN,
               TO_CHAR(C.FECHA_COMPRA,'DD/MM/YYYY HH24:MI') AS FECHA,
               C.TOTAL, C.FORMA_PAGO, C.ESTADO,
               CL.NOMBRE_COMPLETO, CL.EMAIL,
               (SELECT LISTAGG(M.NOMBRE||' x'||DC2.CANTIDAD, ', ')
                FROM DETALLE_COMPRA DC2
                JOIN MUEBLES M ON DC2.ID_MUEBLE = M.ID_MUEBLE
                WHERE DC2.ID_COMPRA = C.ID_COMPRA) AS PRODUCTOS
        FROM COMPRAS C
        JOIN CLIENTES CL ON C.ID_CLIENTE = CL.ID_CLIENTE
        WHERE C.ID_CLIENTE = p_id_cliente
        ORDER BY C.FECHA_COMPRA DESC;
    END PR_HISTORIAL_CLIENTE_ADMIN;

    PROCEDURE PR_CIERRE_CAJAS(
        p_fecha_ini IN DATE, p_fecha_fin IN DATE,
        p_cursor OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT C.FORMA_PAGO,
               COUNT(*)            AS TOTAL_TRANSACCIONES,
               SUM(C.TOTAL)        AS INGRESOS_TOTALES,
               MIN(C.TOTAL)        AS VENTA_MINIMA,
               MAX(C.TOTAL)        AS VENTA_MAXIMA,
               ROUND(AVG(C.TOTAL),2) AS PROMEDIO_VENTA
        FROM COMPRAS C
        WHERE TRUNC(C.FECHA_COMPRA) BETWEEN p_fecha_ini AND p_fecha_fin
          AND C.ESTADO = 'PAGADO'
        GROUP BY C.FORMA_PAGO
        ORDER BY INGRESOS_TOTALES DESC;
    END PR_CIERRE_CAJAS;

    PROCEDURE PR_REPORTE_LTV(p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT CL.ID_CLIENTE, CL.NOMBRE_COMPLETO, CL.EMAIL,
               COUNT(C.ID_COMPRA)          AS TOTAL_COMPRAS,
               NVL(SUM(C.TOTAL), 0)        AS LTV_TOTAL,
               ROUND(NVL(AVG(C.TOTAL),0),2) AS TICKET_PROMEDIO,
               TO_CHAR(MIN(C.FECHA_COMPRA),'DD/MM/YYYY') AS PRIMERA_COMPRA,
               TO_CHAR(MAX(C.FECHA_COMPRA),'DD/MM/YYYY') AS ULTIMA_COMPRA
        FROM CLIENTES CL
        LEFT JOIN COMPRAS C ON CL.ID_CLIENTE = C.ID_CLIENTE
            AND C.ESTADO = 'PAGADO'
        GROUP BY CL.ID_CLIENTE, CL.NOMBRE_COMPLETO, CL.EMAIL
        ORDER BY LTV_TOTAL DESC;
    END PR_REPORTE_LTV;

    PROCEDURE PR_REPORTE_ACTIVIDAD(
        p_fecha_ini IN DATE, p_fecha_fin IN DATE,
        p_cursor OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT TO_CHAR(C.FECHA_COMPRA,'YYYY-MM')  AS MES,
               COUNT(DISTINCT C.ID_CLIENTE)        AS CLIENTES_ACTIVOS,
               COUNT(C.ID_COMPRA)                  AS COMPRAS,
               SUM(C.TOTAL)                        AS INGRESOS
        FROM COMPRAS C
        WHERE TRUNC(C.FECHA_COMPRA) BETWEEN p_fecha_ini AND p_fecha_fin
          AND C.ESTADO = 'PAGADO'
        GROUP BY TO_CHAR(C.FECHA_COMPRA,'YYYY-MM')
        ORDER BY 1;
    END PR_REPORTE_ACTIVIDAD;

    PROCEDURE PR_REPORTE_RETENCION(p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT SEGMENTO, COUNT(*) AS CLIENTES,
               ROUND(COUNT(*)*100.0 / SUM(COUNT(*)) OVER(), 1) AS PORCENTAJE
        FROM (
            SELECT ID_CLIENTE,
                   CASE
                       WHEN COUNT(*) = 1               THEN '1 - Una compra'
                       WHEN COUNT(*) BETWEEN 2 AND 3   THEN '2 - 2 a 3 compras'
                       WHEN COUNT(*) BETWEEN 4 AND 6   THEN '3 - 4 a 6 compras'
                       ELSE                                  '4 - 7 o mas compras'
                   END AS SEGMENTO
            FROM COMPRAS WHERE ESTADO = 'PAGADO'
            GROUP BY ID_CLIENTE
        )
        GROUP BY SEGMENTO
        ORDER BY SEGMENTO;
    END PR_REPORTE_RETENCION;

    PROCEDURE PR_REPORTE_COHORTE(p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT TO_CHAR(PRIMERA_COMPRA,'YYYY-MM') AS COHORTE,
               COUNT(DISTINCT ID_CLIENTE)         AS CLIENTES_NUEVOS,
               SUM(TOTAL_COMPRAS)                 AS COMPRAS_TOTALES,
               ROUND(AVG(LTV), 2)                 AS LTV_PROMEDIO
        FROM (
            SELECT ID_CLIENTE,
                   MIN(FECHA_COMPRA) AS PRIMERA_COMPRA,
                   COUNT(*)          AS TOTAL_COMPRAS,
                   SUM(TOTAL)        AS LTV
            FROM COMPRAS WHERE ESTADO = 'PAGADO'
            GROUP BY ID_CLIENTE
        )
        GROUP BY TO_CHAR(PRIMERA_COMPRA,'YYYY-MM')
        ORDER BY 1;
    END PR_REPORTE_COHORTE;

    PROCEDURE PR_REPORTE_REMARKETING(p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
        SELECT CL.ID_CLIENTE, CL.NOMBRE_COMPLETO, CL.EMAIL,
               TO_CHAR(CA.FECHA_CREACION,'DD/MM/YYYY') AS FECHA_CARRITO,
               COUNT(DC.ID_DETALLE_CARRITO)            AS ITEMS_EN_CARRITO,
               SUM(DC.SUBTOTAL)                        AS VALOR_CARRITO
        FROM CARRITO CA
        JOIN DETALLE_CARRITO DC ON CA.ID_CARRITO = DC.ID_CARRITO
        JOIN CLIENTES CL        ON CA.ID_CLIENTE = CL.ID_CLIENTE
        WHERE CA.ESTADO = 'ACTIVO'
        GROUP BY CL.ID_CLIENTE, CL.NOMBRE_COMPLETO, CL.EMAIL, CA.FECHA_CREACION
        ORDER BY VALOR_CARRITO DESC;
    END PR_REPORTE_REMARKETING;

END PKG_REPORTES;
/

COMMIT;
PROMPT =============================================
PROMPT  Todos los paquetes creados exitosamente
PROMPT  PKG_MUEBLES + PKG_CLIENTES + PKG_REPORTES
PROMPT =============================================
