﻿using GerenciadorDeFestas.Dominio.ModuloAluguel;
using GerenciadorDeFestas.Dominio.ModuloCliente;
using GerenciadorDeFestas.Infra.Dados.Arquivo.Compartilhado;
using GerenciadorDeFestas.WinForms.Compartilhado;
using GerenciadorDeFestas.WinForms.ModuloAluguel;

namespace GerenciadorDeFestas.WinForms.ModuloCliente
{
    public class ControladorCliente : ControladorBase
    {
        private IRepositorioCliente repositorioCliente;
        private TabelaClienteControl tabelaCliente;
        private TabelaAluguelControl tabelaAluguel;

        public ControladorCliente(IRepositorioCliente repositorioCliente)
        {
            this.repositorioCliente = repositorioCliente;
        }

        #region
        public override string ToolTipInserir { get { return "Inserir novo Cliente"; } }

        public override string ToolTipEditar { get { return "Editar Cliente existente"; } }

        public override string ToolTipExcluir { get { return "Excluir Cliente existente"; } }

        public override string ToolTipVisualizar { get { return "Visualizar Alugueis do Cliente"; } }

        public override bool PagamentoHabilitado => false;
        #endregion

        public override void Inserir()
        {
            TelaClienteForm telaCliente = new TelaClienteForm(repositorioCliente.SelecionarTodos());

            DialogResult opcaoEscolhida = telaCliente.ShowDialog();

            if (opcaoEscolhida == DialogResult.OK)
            {
                Cliente cliente = telaCliente.ObterCliente();

                repositorioCliente.Inserir(cliente);

                CarregarClientes();
            }
        }

        public override void Editar()
        {
            Cliente clienteSelecionado = ObterClienteSelecionado();

            if (clienteSelecionado == null)
            {
                ApresentarMensagem("Selecione um cliente primeiro!", "Edição de Clientes");
                return;
            }

            TelaClienteForm telaCliente = new TelaClienteForm(repositorioCliente.SelecionarTodos());
            telaCliente.ConfigurarTela(clienteSelecionado);

            DialogResult opcaoEscolhida = telaCliente.ShowDialog();

            if (opcaoEscolhida == DialogResult.OK)
            {
                Cliente cliente = telaCliente.ObterCliente();

                repositorioCliente.Editar(cliente.id, cliente);

                CarregarClientes();
            }
        }

        public override void Excluir()
        {
            Cliente cliente = ObterClienteSelecionado();

            if (cliente == null)
            {
                ApresentarMensagem("Selecione um cliente primeiro!", "Exclusão de Clientes");
                return;
            }


            DialogResult opcaoEscolhida = MessageBox.Show($"Deseja excluir o cliente {cliente.nome}?", "Exclusão de Clientes",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (opcaoEscolhida == DialogResult.OK)
            {
                if(cliente.alugueis.Count > 0)
                {
                    MessageBox.Show("Exclusão inválida, cliente possui aluguéis");
                    return;
                }

                repositorioCliente.Excluir(cliente);
            }

            CarregarClientes();
        }

        private Cliente ObterClienteSelecionado()
        {
            int id = tabelaCliente.ObterIdSelecionado();

            return repositorioCliente.SelecionarPorId(id);
        }

        private void CarregarClientes()
        {
            List<Cliente> clientes = repositorioCliente.SelecionarTodos();

            tabelaCliente.AtualizarRegistros(clientes);
        }

        public override UserControl ObterListagem()
        {
            if (tabelaCliente == null)
                tabelaCliente = new TabelaClienteControl();

            CarregarClientes();

            return tabelaCliente;
        }

        public override string ObterTipoCadastro()
        {
            return "Cadastro de Clientes";
        }

        public override void Visualizar()
        {
            Cliente cliente = ObterClienteSelecionado();

            TelaVisualizarAlugueisForm telaListagemAlugueis = new TelaVisualizarAlugueisForm(cliente);

            if (tabelaAluguel == null)
                tabelaAluguel = new TabelaAluguelControl();

            if (cliente == null)
            {
                ApresentarMensagem("Selecione um cliente primeiro!", "Listagem de alugueis");
                return;
            }

            telaListagemAlugueis.CarregarLabel(cliente);

            telaListagemAlugueis.CarregarRegistros(cliente.alugueis);

            telaListagemAlugueis.ShowDialog();
        }

        public override void ApresentarMensagem(string mensagem, string titulo)
        {
            MessageBox.Show(mensagem, titulo, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
