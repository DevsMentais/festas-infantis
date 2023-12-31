﻿using GerenciadorDeFestas.Dominio.ModuloAluguel;
using GerenciadorDeFestas.WinForms.Compartilhado;

namespace GerenciadorDeFestas.WinForms.ModuloAluguel
{
    public partial class TabelaAluguelControl : UserControl
    {
        public TabelaAluguelControl()
        {
            InitializeComponent();

            ConfigurarColunas();

            grid.ConfigurarGridZebrado();

            grid.ConfigurarGridSomenteLeitura();
        }

        public void AtualizarRegistros(List<Aluguel> alugueis)
        {
            grid.Rows.Clear();

            foreach (Aluguel aluguel in alugueis)
            {
                grid.Rows.Add(aluguel.id, 
                aluguel.cliente, aluguel.tema,
                aluguel.data.ToString("dd/MM/yyyy"), 
                aluguel.dataFechamento == new DateTime() ? "Em Aberto" :
                aluguel.dataFechamento.ToString("dd/MM/yyyy"), 
                aluguel.desconto,
                aluguel.porcentagemPaga + "%", 
                aluguel.valorAhPagar.ToString("F2"));
            }
            TelaPrincipalForm.Instancia.AtualizarRodape($"Visualizando {alugueis.Count} alugueis");
        }

        private void ConfigurarColunas()
        {
            DataGridViewColumn[] colunas = new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn()
                {
                    Name = "id",
                    HeaderText = "Id"
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "cliente",
                    HeaderText = "Cliente"
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "tema",
                    HeaderText = "Tema"
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "data",
                    HeaderText = "Data"
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "dataFechamento",
                    HeaderText = "Fechamento"
                },       new DataGridViewTextBoxColumn()
                {
                    Name = "desconto",
                    HeaderText = "Desconto"
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "pago",
                    HeaderText = "Entrada"
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "valorAhPagar",
                    HeaderText = "Valor a Pagar"
                }
            };
            grid.Columns.AddRange(colunas);
        }

        public int ObterIdSelecionado()
        {
            int id;

            try
            {
                id = Convert.ToInt32(grid.SelectedRows[0].Cells["id"].Value);
            }
            catch
            {
                id = -1;
            }

            return id;
        }

    }
}