$().ready(function ()
{
    const table = document.getElementById("tableMedidasTR");
    var tableMedidas = "";

    window.chrome.webview.addEventListener('message', event => {
        const data = event.data;
        if (data.tipo === "actualizarMedidas") {
            LoadMedidas(data.medidas);           
        }
    });

    if (!$("#Err").val() == "" || !$("#Err").val() != null) {
        document.getElementById("closeErr").style.visibility = "hidden";
    }

    window.chrome.webview.postMessage({
        tipo: "actualizarPAC",
        pacienteId: $("#Paciente").val()
    });    
    
    tableMedidas = $('#tableMedidas').DataTable();

    if ($("#AplicaEncuesta").val() == "SI")
    {
        window.chrome.webview.postMessage({
            tipo: "encuesta",
            admision: $("#textBox1").val()
        });
    }

    $("#btnAddHerida").click(function ()
    {
        window.chrome.webview.postMessage({
            tipo: "addHerida",
            admision: $("#textBox1").val(),
            profundidad: $("#txtProfundidad").val(),
            largo: $("#txtLargo").val(),
            ancho: $("#txtAncho").val(),
            selEvolucionHerida: $("#selEvolucionHerida").val(),
            SinEvolucion: $("#SinEvolucion").val(),
            selEstadoHerida: $("#selEstadoHerida").val(),
            OtroEstado: $("#OtroEstado").val(),
            NovedadHerida: $("#NovedadHerida").val()
        });

        window.chrome.webview.postMessage({
            tipo: "consultaHerida",
            admision: $("#textBox1").val()
        });
    });

    function LoadMedidas(medidas) {
        const tbody = document.getElementById("tableMedidasTR");
        tbody.innerHTML = ""; // limpiar contenido anterior

        medidas.forEach(m => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
            <td>${m.Id}</td>
            <td>${m.Profundidad}</td>
            <td>${m.Ancho}</td>
            <td>${m.Largo}</td>
            <td>${m.Total}</td>
        `;
            tbody.appendChild(tr);
        });       
    }

    $("#closeErr").click(function ()
    {
        window.chrome.webview.postMessage("cerrarFormulario");
    });
    $("#btnDatoPaciente").click(function ()
    {
        window.chrome.webview.postMessage({
            tipo: "actualizarPAC",
            pacienteId: $("#Paciente").val()
        });
    });
    $("#btnAdherencia").click(function () {
        window.chrome.webview.postMessage({
            tipo: "adherenciaApositos"
        });
    });
    $("#btnHC").click(function () {
        window.chrome.webview.postMessage({
            tipo: "historiaClinica"
        });
    });
    $("#btnResumenHC").click(function () {
        window.chrome.webview.postMessage({
            tipo: "resumenHistoria",
            pacienteId: $("#Paciente").val()
        });
    });
    $("#btnCManejo").click(function () {
        window.chrome.webview.postMessage({
            tipo: "cManejo",
            nId: $("#CMANID").val(),
            tId: $("#CMANTID").val()
        });
    });
    $("#btnTraerUltima").click(function () {
        window.chrome.webview.postMessage({
            tipo: "tUltima",
            pacienteId: $("#Paciente").val()
        });
    });
    $("#btnCargaPlantilla").click(function () {
        window.chrome.webview.postMessage({
            tipo: "cPlantilla"
        });
    });
    $("#btnTratamiento").click(function () {
        window.chrome.webview.postMessage({
            tipo: "tratamiento",
            pacienteId: $("#Paciente").val()
        });
    });
    $("#btnMensajero").click(function () {
        window.chrome.webview.postMessage({
            tipo: "mensajero"
        });
    });
    $("#btnCancelar").click(function () {
        window.chrome.webview.postMessage({
            tipo: "cancelar",
            admision: $("#textBox1").val()
        });
    });
    $("#btnNoApositos").click(function () {
        $("#richTextBox1").val("NO SE USARON APOSITOS EN ESTA CURACION");     
    });

    $("#btnCleanDX1").click(function () {
        $("#textBox6").val("");
        $("#textBox7").val("");
    });
    $("#btnCleanDX2").click(function () {
        $("#textBox8").val("");
        $("#textBox10").val("");
    });
    $("#btnCleanDX3").click(function () {
        $("#textBox9").val("");
        $("#textBox11").val("");
    });

    $("#textBox6").click(function () {
        window.chrome.webview.postMessage({
            tipo: "DX1"
        });
    });
    $("#textBox8").click(function () {
        window.chrome.webview.postMessage({
            tipo: "DX2"
        });
    });
    $("#textBox9").click(function () {
        window.chrome.webview.postMessage({
            tipo: "DX3"
        });
    });

    $("#btnGrabar").click(function () {
        window.chrome.webview.postMessage({
            tipo: "grabar",
            admision: $("#textBox1").val(),
            textBox3: $("#textBox3").val(),
            textBox2: $("#textBox2").val(),
            textBox4: $("#textBox4").val(),
            textBox9: $("#textBox9").val(),
            textBox8: $("#textBox8").val(),
            textBox13: $("#textBox13").val(),
            textBox12: $("#textBox12").val(),
            textBox14: $("#textBox14").val(),
            textBox15: $("#textBox15").val(),
            textBox16: $("#textBox16").val(),
            textBox18: $("#textBox18").val(),
            textBox24: $("#textBox24").val(),
            textBox6: $("#textBox6").val(),
            textBox19: $("#textBox19").val(),
            richTextBox1: $("#richTextBox1").val(),
            selTraerHistoria: $("#selTraerHistoria").val(),
            checkBox5: $("#checkBox5").val(),
            paciente: $("#Paciente").val(),
            cia: $("#Cia").val(),
            ase: $("#Ase").val(),
            prof: $("#Prof").val(),
            Fecha_Serv: $("#Fecha_Serv").val(),
            CUP: $("#CUP").val(),
            TSERV: $("#TSERV").val(),
            Valor: $("#Valor").val(),
            Serv: $("#Serv").val(),
            Reg_RIP: $("#Reg_RIP").val(),
            selDobleEspacio: $("#selDobleEspacio").val(),
            selDosVecesSemana: $("#selDosVecesSemana").val(),
            selCondicionEspecial: $("#selCondicionEspecial").val(),
            VIH: $("#selVIH").val(),
            Hepatitis: $("#selHepatitis").val()           
        });
    });
    
    function setAdherenciaTexto(texto) {
        const box = document.getElementById('richTextBox1');
        if (box) {
            box.value = texto;
        } else {
            // Si el control aún no existe, reintenta cada 200 ms
            setTimeout(() => setAdherenciaTexto(texto), 200);
        }
    }
    $("#btnCleanAdAp").click(function () {
        $("#richTextBox1").val("");
    });

    $("#btnCaida").click(function ()
    {
        Conditions("CAIDA");
    });
    $("#btnInfeccion").click(function () {
        Conditions("INFECCION");
    });
    $("#btnDeterioro").click(function () {
        Conditions("DETERIORO DE LA PIEL");
    });
    $("#btnAlergia").click(function () {
        Conditions("ALERGIA");
    });
    $("#btnComunicacion").click(function () {
        Conditions("DIFICULTAD DE COMUNICACION");
    });
    $("#btnPsiquiatrico").click(function () {
        Conditions("PACIENTE PSIQUIATRICO");
    });
    $("#btnMayor").click(function () {
        Conditions("MAYOR DE 70 AÑOS");
    });
    $("#btnDificil").click(function () {
        Conditions("PACIENTE DIFICIL");
    });
    $("#btnMedico").click(function () {
        Conditions("MEDICO LO REQUIERE");
    });
    function Conditions(tipos) {
        window.chrome.webview.postMessage({
            tipo: "opcionesRecomendaciones",
            clase: tipos.toString(),
            pacienteId: $("#Paciente").val()
        });
    }

    table.addEventListener("dblclick", function (event) {
        const tr = event.target.closest("tr");
        if (!tr) return; // clic fuera de fila

        const id = tr.children[0].textContent; // Columna 0 = Id

        if (confirm("¿Desea eliminar esta medida de identificador único " + id + "?")) {
            window.chrome.webview.postMessage({
                tipo: "eliminarMedida",
                id: id
            });

            window.chrome.webview.postMessage({
                tipo: "consultaHerida",
                admision: document.getElementById("textBox1").value
            });

            alert("Medida Eliminada");
        }
    });

    
});