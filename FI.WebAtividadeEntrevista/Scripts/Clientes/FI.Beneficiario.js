
$(document).ready(function () {
    debugger;
    beneficiariosClienteAtual = []
    if (obj) {
        if (obj.Beneficiarios.length > 0) {
            beneficiariosClienteAtual = obj.Beneficiarios;            
        }
    }

    let modoEdicao = false;
    let cpfOriginalEmEdicao = null;
    const tabelaBody = $('#tabelaBeneficiariosBody');
    const btnIncluir = $('#btnIncluirBeneficiario');
    const cpfInput = $('#cpfNovoBeneficiario');
    const nomeInput = $('#nomeNovoBeneficiario');
    const btnBeneficiariosModal = $('#btnBeneficiariosModal');

    btnBeneficiariosModal.on('click', function () {
        gerarGridBeneficiarios();
    })


    btnIncluir.on('click', function () {
        debugger;
        const cpfLimpo = cpfInput.val().replace(/\D/g, '');
        const nome = nomeInput.val().trim();


        const indice = beneficiariosClienteAtual.findIndex(b => b.CPF === cpfLimpo);

        if (indice !== -1) {

            beneficiariosClienteAtual[indice].CPF = cpfLimpo;
            beneficiariosClienteAtual[indice].Nome = nome;

            gerarGridBeneficiarios();
        } else {

            const novoId = -(beneficiariosClienteAtual.length + 1);

            beneficiariosClienteAtual.push({
                Id: novoId,
                CPF: cpfLimpo,
                Nome: nome
            });

            gerarGridBeneficiarios();
        }
    })


    tabelaBody.on('click', '.btn-excluir', function () {
        const row = $(this).closest('tr');
        const cpfExcluir = row.data('cpf');

        if (confirm('Tem certeza que deseja remover este beneficiário?')) {


            const novaLista = beneficiariosClienteAtual.filter(b => b.CPF !== cpfExcluir);

            beneficiariosClienteAtual = novaLista;         

            gerarGridBeneficiarios();
        }
    });



    tabelaBody.on('click', '.btn-alterar', function () {
        debugger;
        const cpfLimpo = cpfInput.val().replace(/\D/g, '');
        const nome = nomeInput.val().trim();

        const indice = beneficiariosClienteAtual.findIndex(b => b.CPF === cpfLimpo);

        if (indice !== -1) {

            beneficiariosClienteAtual[indice].CPF = cpfLimpo;
            beneficiariosClienteAtual[indice].Nome = nome;
            gerarGridBeneficiarios();
        }        
    });

    function formatarCpfParaExibicao(cpf) {
        cpf = cpf.replace(/\D/g, '');
        if (cpf.length === 11) {
            return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
        }
        return cpf;
    }


    function gerarGridBeneficiarios() {

        tabelaBody.empty();


        if (!beneficiariosClienteAtual || beneficiariosClienteAtual.length === 0) {
            tabelaBody.append('<tr><td colspan="3" class="text-center">Nenhum beneficiário cadastrado.</td></tr>');
            return;
        }


        beneficiariosClienteAtual.forEach(function (beneficiario) {

            const cpfFormatado = formatarCpfParaExibicao(beneficiario.CPF);

            const newRow = `
                <tr data-id="${beneficiario.Id}" data-cpf="${beneficiario.CPF}">
                    <td>${cpfFormatado}</td>
                    <td>${beneficiario.Nome}</td>
                    <td class="text-center">
                        <button type="button" class="btn btn-primary btn-sm btn-alterar">Alterar</button>
                        <button type="button" class="btn btn-danger btn-sm btn-excluir">Excluir</button>
                    </td>
                </tr>
            `;


            tabelaBody.append(newRow);
        });
    }
})

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}

