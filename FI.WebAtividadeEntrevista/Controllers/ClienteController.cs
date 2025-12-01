using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebAtividadeEntrevista.Models;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente boCliente = new BoCliente();
            BoBeneficiario boBeneficiario = new BoBeneficiario();

            model.CPF = Regex.Replace(model.CPF, "[^0-9]", "");
            var cpfValido = ValidaCpf(model.CPF);
            if (!cpfValido)
            {                
                return Json( new {Success = false, Message ="❌ CPF inválido. Por favor, verifique o CPF."});
            }

            Cliente cliente = boCliente.Consultar(cpf: model.CPF);

            if (cliente != null)
            {
                return Json(new { Success = false, Message = "❌ CPF já cadastrado" });
            }

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                
                model.Id = boCliente.Incluir(new Cliente()
                {
                    CPF = model.CPF,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone
                });


                foreach (var beneficiario in model.Beneficiarios)
                {
                    var beneficiarioCpfValido = ValidaCpf(beneficiario.CPF);
                    if (!beneficiarioCpfValido)
                    {
                        return Json(new { Success = false, Message = "❌ CPF beneficiario inválido. Por favor, verifique o CPF." });
                    }

                    List<Beneficiario> beneficiarios = boBeneficiario.Consultar(model.Id, beneficiario.CPF);

                    if (beneficiarios.Any())
                    {
                        return Json(new { Success = false, Message = "❌ CPF já cadastrado para beneficiario" });
                    }

                    boBeneficiario.Incluir(new Beneficiario()
                    {
                        CPF = beneficiario.CPF,
                        Nome = beneficiario.Nome,
                        IdCliente = model.Id
                    });
                }

                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {

            model.CPF = Regex.Replace(model.CPF, "[^0-9]", "");
            var cpfValido = ValidaCpf(model.CPF);
            if (!cpfValido)
            {
                return Json(new { Success = false, Message = "❌ CPF inválido. Por favor, verifique o numero." });
            }

            BoCliente boCliente = new BoCliente();
            BoBeneficiario boBeneficiario = new BoBeneficiario();



            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                boCliente.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CPF = model.CPF,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone
                });

                boBeneficiario.Excluir(model.Id);
                bool cpfBenificiarioCadastrado = false;
                foreach (var beneficiario in model.Beneficiarios)
                {
                    var beneficiarioCpfValido = ValidaCpf(beneficiario.CPF);
                    if (!beneficiarioCpfValido)
                    {
                        return Json(new { Success = false, Message = "❌ CPF beneficiario inválido. Por favor, verifique o CPF." });
                    }

                    List<Beneficiario> beneficiarios = boBeneficiario.Consultar(model.Id, beneficiario.CPF);
                    
                    if (beneficiarios.Any())
                    {
                        cpfBenificiarioCadastrado = true;
                    }
                    else
                    {
                        boBeneficiario.Incluir(new Beneficiario()
                        {
                            CPF = beneficiario.CPF,
                            Nome = beneficiario.Nome,
                            IdCliente = model.Id
                        });
                    }
                }
                if (cpfBenificiarioCadastrado)
                {
                    return Json(new { Success = false, Message = "❌ CPF já cadastrado para beneficiario" });
                }

                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;
          
                List<Beneficiario> beneficiario = new BoBeneficiario().Consultar(cliente.Id, "");                

                cliente.Beneficiarios = beneficiario;
            

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CPF = cliente.CPF,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    Beneficiarios = cliente.Beneficiarios
                };

            
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);
               
                


                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private static bool ValidaCpf(string cpf)
        {            
            string cpfLimpo = new Regex(@"[^\d]").Replace(cpf, string.Empty);

         
            if (cpfLimpo.Length != 11)
                return false;

            if (cpfLimpo.Distinct().Count() == 1)
                return false;

         
            int[] multiplicadores1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicadores2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpfLimpo.Substring(0, 9);
            int soma = 0;

         
            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
                     
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();
            
            return cpfLimpo.EndsWith(digito);
        }
    }
}