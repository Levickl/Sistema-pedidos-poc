import React, { useEffect, useState } from "react";
import axios from "axios";

export default function App() {
  const [pedidos, setPedidos] = useState([]);
  const [form, setForm] = useState({ cliente: "", produto: "", valor: "" });
  const [detalhes, setDetalhes] = useState(null);

  const fetchPedidos = async () => {
    const res = await axios.get("http://localhost:5109/api/orders");
    setPedidos(res.data);
  };

  const criarPedido = async (e) => {
    e.preventDefault();
    console.log("Entrou")
    await axios.post("http://localhost:5109/api/orders", {
      cliente: form.cliente,
      produto: form.produto,
      valor: parseFloat(form.valor)
    }).then(()=>{
      
    }).catch((e)=>{
      console.log(e);
    });
    setForm({ cliente: "", produto: "", valor: "" });
    fetchPedidos();
  };

  useEffect(() => {
    fetchPedidos();
  }, []);

  return (
    <div className="p-4 max-w-4xl mx-auto">
      <h1 className="text-2xl font-bold mb-4">Sistema de Pedidos</h1>

      <form onSubmit={criarPedido} className="mb-6 grid grid-cols-1 md:grid-cols-3 gap-4">
        <input
          type="text"
          placeholder="Cliente"
          className="border p-2 rounded"
          value={form.cliente}
          onChange={(e) => setForm({ ...form, cliente: e.target.value })}
          required
        />
        <input
          type="text"
          placeholder="Produto"
          className="border p-2 rounded"
          value={form.produto}
          onChange={(e) => setForm({ ...form, produto: e.target.value })}
          required
        />
        <input
          type="number"
          placeholder="Valor"
          className="border p-2 rounded"
          value={form.valor}
          onChange={(e) => setForm({ ...form, valor: e.target.value })}
          required
        />
        <button className="bg-blue-500 text-white rounded p-2 col-span-1 md:col-span-3">
          Criar Pedido
        </button>
      </form>

      <table className="w-full table-auto border text-sm">
        <thead>
          <tr className="bg-gray-100 text-black">
            <th className="border p-2">Cliente</th>
            <th className="border p-2">Produto</th>
            <th className="border p-2">Valor</th>
          </tr>
        </thead>
        <tbody>
          {pedidos.map((p) => (
            <tr
              key={p.id}
              className="hover:bg-gray-50 hover:text-black cursor-pointer"
              onClick={() => setDetalhes(p)}
            >
              <td className="border p-2">{p.cliente}</td>
              <td className="border p-2">{p.produto}</td>
              <td className="border p-2">R$ {p.valor.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>

      {detalhes && (
        <div className="mt-6 p-4 border rounded bg-gray-50 text-black">
          <h2 className="text-lg font-bold mb-2">Detalhes do Pedido</h2>
          <p><strong>Cliente:</strong> {detalhes.cliente}</p>
          <p><strong>Produto:</strong> {detalhes.produto}</p>
          <p><strong>Valor:</strong> R$ {detalhes.valor.toFixed(2)}</p>
          <p><strong>Status:</strong> {detalhes.status}</p>
          <p><strong>Data de Criação:</strong> {new Date(detalhes.dataCriacao).toLocaleString()}</p>
          <button className="mt-2 text-sm text-blue-500" onClick={() => setDetalhes(null)}>Fechar</button>
        </div>
      )}
    </div>
  );
}