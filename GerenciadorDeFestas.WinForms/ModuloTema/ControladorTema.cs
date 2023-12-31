﻿using GerenciadorDeFestas.Dominio.ModuloAluguel;
using GerenciadorDeFestas.Dominio.ModuloCliente;
using GerenciadorDeFestas.Dominio.ModuloItem;
using GerenciadorDeFestas.Dominio.ModuloTema;
using GerenciadorDeFestas.WinForms.Compartilhado;
using GerenciadorDeFestas.WinForms.ModuloAluguel;

namespace GerenciadorDeFestas.WinForms.ModuloTema
{
    public class ControladorTema : ControladorBase
    {
        private IRepositorioItem repositorioItem;
        private IRepositorioTema repositorioTema;
        private TabelaTemaControl tabelaTemaControl;

        public ControladorTema(IRepositorioItem repositorioItem, IRepositorioTema repositorioTema)
        {
            this.repositorioItem = repositorioItem;
            this.repositorioTema = repositorioTema;
        }

        #region
        public override string ToolTipInserir { get { return "Inserir Tema"; } }

        public override string ToolTipEditar { get { return "Editar Tema existente"; } }

        public override string ToolTipExcluir { get { return "Excluir Tema existente"; } }

        public override string ToolTipVisualizar { get { return "Visualizar itens do Tema"; } }

        public override bool PagamentoHabilitado => false;
        #endregion

        public override void Inserir()
        {
            TelaTemaForm telaTemaForm = new TelaTemaForm(repositorioItem.SelecionarTodos(), repositorioTema.SelecionarTodos());

            DialogResult opcaoEscolhida = telaTemaForm.ShowDialog();

            if (opcaoEscolhida == DialogResult.OK)
            {
                Tema tema = telaTemaForm.ObterTema();

                for (int i = 0; i < tema.itens.Count; i++)
                {
                    tema.itens[i].temas.Add(tema);
                }

                decimal valor = tema.CalcularValor();

                tema.resultado = valor;

                repositorioTema.Inserir(tema);

                CarregarTemas();
            }
        }

        public override void Editar()
        {
            Tema temaSelecionado = ObterTemaSelecionado();

            if (temaSelecionado == null)
            {
                ApresentarMensagem("Selecione um tema primeiro!", "Edição de Temas");
                return;
            }

            TelaTemaForm telaTema = new TelaTemaForm(repositorioItem.SelecionarTodos(), repositorioTema.SelecionarTodos());
            telaTema.ConfigurarTela(temaSelecionado);

            DialogResult opcaoEscolhida = telaTema.ShowDialog();

            if (opcaoEscolhida == DialogResult.OK)
            {
                Tema tema = telaTema.ObterTema();

                repositorioTema.Editar(tema.id, tema);

                CarregarTemas();
            }
        }

        public override void Excluir()
        {
            Tema tema = ObterTemaSelecionado();

            if (tema == null)
            {
                ApresentarMensagem("Selecione um tema primeiro!", "Exclusão de Temas");
                return;
            }

            DialogResult opcaoEscolhida = MessageBox.Show($"Deseja excluir o tema {tema.nome}?", "Exclusão de Temas",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);


            if (opcaoEscolhida == DialogResult.OK)
            {
                for (int i = 0; i < tema.itens.Count(); i++)
                {
                    tema.itens[i].temas.Remove(tema);
                }

                if (tema.alugueis.Count > 0)
                {
                    MessageBox.Show("Exclusão inválida, tema possui aluguéis");
                    return;
                }

                repositorioTema.Excluir(tema);

                CarregarTemas();
            }
        }

        private Tema ObterTemaSelecionado()
        {
            int id = tabelaTemaControl.ObterTemaSelecionado();

            return repositorioTema.SelecionarPorId(id);
        }

        private void CarregarTemas()
        {
            List<Tema> temas = repositorioTema.SelecionarTodos();

            tabelaTemaControl.AtualizarRegistros(temas);
        }

        public override UserControl ObterListagem()
        {
            if (tabelaTemaControl == null)
                tabelaTemaControl = new TabelaTemaControl();

            CarregarTemas();

            return tabelaTemaControl;
        }

        public override string ObterTipoCadastro()
        {
            return "Cadastro de Temas";
        }

        public override void Visualizar()
        {
            Tema tema = ObterTemaSelecionado();

            TelaVisualizarItensForm telaListagem = new TelaVisualizarItensForm();

            if (tema == null)
            {
                ApresentarMensagem("Selecione um tema primeiro!", "Listagem de itens");
                return;
            }

            telaListagem.CarregarLabel(tema);

            telaListagem.CarregarLista(tema.itens);

            telaListagem.ShowDialog();
        }

        public override void ApresentarMensagem(string mensagem, string titulo)
        {
            MessageBox.Show(mensagem, titulo, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
