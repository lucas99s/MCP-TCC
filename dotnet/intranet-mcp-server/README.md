# 🧪 MCP Server para Validação de Interação com Ferramentas em LLMs

## 📌 Sobre o Projeto

Este repositório contém a implementação de um **MCP Server (Model Context Protocol)** desenvolvido como parte do Trabalho de Conclusão de Curso (TCC) do MBA em Engenharia de Software da USP/ESALQ.

O objetivo principal deste projeto é **avaliar experimentalmente como modelos de linguagem (LLMs) interagem com ferramentas externas**, com foco em:

* Identificação correta da ferramenta
* Seleção adequada da ferramenta
* Invocação correta da ferramenta

A pesquisa está inserida em um contexto de **validação de arquiteturas baseadas em MCP para integração de microsserviços heterogêneos**, alinhando teoria e prática — conforme esperado em um TCC de natureza aplicada .

---

## 🎯 Objetivo do MCP Server

Este MCP Server foi projetado especificamente para suportar **cenários experimentais controlados**, permitindo analisar o impacto de três variáveis principais:

1. 🧠 Modelo de linguagem (LLM)
2. 🧾 Qualidade semântica das ferramentas (títulos e descrições).
3. 💬 Qualidade do prompt do usuário

O foco deste repositório está na **variável "qualidade da descrição das ferramentas"**, onde são testadas duas abordagens:

* ✅ Descrições detalhadas e semânticas
* ⚠️ Descrições genéricas e ambíguas

---

## 🎯 Qualidade semântica das ferramentas.

Para fins deste experimento, a qualidade da descrição da ferramenta foi tratada de forma abrangente, incluindo não apenas a descrição textual principal, mas também o nome da tool e as descrições de seus parâmetros. 
Essa decisão foi adotada porque, na interação entre modelos de linguagem e ferramentas MCP, todos esses elementos compõem conjuntamente a superfície semântica utilizada pelo modelo para inferir a função da ferramenta e decidir sua seleção.

---

## 🔬 Hipótese da Pesquisa

A hipótese central deste experimento é:

> *Descrições mais claras, estruturadas e semanticamente ricas aumentam a capacidade do modelo de linguagem de selecionar e utilizar corretamente ferramentas via MCP.*

---

## 🧩 Estrutura do Projeto

O projeto é implementado em **.NET** e segue uma arquitetura modular para facilitar testes e extensibilidade.

### 📁 Componentes principais:

* **Tools (Ferramentas MCP)**

  * Implementações reais de operações simulando uma intranet de uma empresa.

* **Tool Descriptions**

  * Versão A: descrições detalhadas (alta qualidade)
  * Versão B: descrições genéricas (baixa qualidade)

* **MCP Server**

  * Responsável por expor as ferramentas para consumo por LLMs

* **Cenários de Teste**

  * Conjunto de prompts utilizados para avaliar comportamento da IA

* **Camada de avaliação planejada**

  * Registro de resultados:

	* Tools escolhidas
	* Tempo de execução
	* Quantidade de tokens utilizados
	* Número de chamadas de ferramentas
	* Modelo utilizado
	* Tipo de prompt (claro ou ambíguo)
	* Tipo de descrição (detalhada ou genérica)

---

## ⚙️ Cenário Experimental

Os testes realizados com este servidor seguem uma abordagem de **pesquisa experimental**, onde variáveis são manipuladas para observar seus efeitos.

### 🔁 Combinações avaliadas:

| Variável          | Opções                   |
| ----------------- | ------------------------ |
| LLM               | Modelo grande vs pequeno |
| Descrição da Tool | Detalhada vs Genérica    |
| Prompt            | Claro vs Ambíguo         |

---

## 🧠 Importância da Pesquisa

A maioria das implementações atuais de IA foca apenas em:

* Prompt Engineering
* Escolha do modelo

Este trabalho investiga um ponto ainda pouco explorado:

> 📌 **A qualidade semântica das ferramentas como fator crítico na tomada de decisão da IA**

---

## 🏗️ Motivação Técnica

Em arquiteturas modernas baseadas em:

* MCP
* Tool Calling
* Agents
* Semantic Kernel
* LangChain

A IA precisa decidir:

1. Qual ferramenta usar
2. Quando usar
3. Como usar

Se a descrição da ferramenta for ruim, o sistema pode falhar.

---

## 🚀 Aplicações Práticas

Os resultados deste projeto podem impactar diretamente:

* Desenvolvimento de agentes inteligentes
* Sistemas multi-tool (ex: copilots corporativos)
* Arquiteturas baseadas em MCP
* Integrações com microserviços

---

## 📊 Métricas Avaliadas

Durante os testes, serão coletadas métricas como:

* 🎯 Taxa de acerto na escolha da tool
* 🔁 Número de tentativas até sucesso
* ⚠️ Uso incorreto de ferramentas
* 🧠 Interpretação do prompt

---

## 📚 Contexto Acadêmico

Este projeto faz parte de um TCC de natureza aplicada, cujo objetivo é:

* Resolver um problema real
* Aplicar metodologia científica
* Gerar conhecimento técnico relevante 

A pesquisa utiliza abordagem experimental, permitindo observar relações de causa e efeito entre variáveis.

---

## 🛠️ Tecnologias Utilizadas

* .NET (C#)
* MCP (Model Context Protocol)
* LLMs (GPT-5.4, GPT-5 nano)
* Docker
* Logging / Observabilidade

---

## 🔮 Próximos Passos

* [X] Implementar tools semanticamente ricas
* [X] Implementar tools semanticamente genéricas
* [ ] Implementar execução automatizada dos cenários de teste
* [ ] Coletar métricas experimentais
* [ ] Gerar dataset de resultados
* [ ] Produzir visualizações e análises

---

## 👨‍💻 Autor

Lucas Rodrigues
MBA em Engenharia de Software – USP/ESALQ

---

## 📌 Observação

Este projeto não é apenas uma implementação técnica, mas sim um **ambiente experimental controlado**, utilizado para validar hipóteses sobre o comportamento de LLMs em arquiteturas baseadas em ferramentas.
