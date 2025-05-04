// src/pages/Home.tsx
import React, { useEffect, useState } from 'react';
import { Imovel } from '../types/Imovel';
import api from '../services/api';

const Home = () => {
  const [imoveis, setImoveis] = useState<Imovel[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchImoveis = async () => {
      try {
        const response = await api.get('/Imoveis');
        setImoveis(response.data);
      } catch (error) {
        console.error("Erro ao carregar imóveis:", error);
      } finally {
        setLoading(false);
      }
    };
    fetchImoveis();
  }, []);

  if (loading) {
    return <div className="text-center">Carregando imóveis...</div>;
  }

  return (
    <div className="container mt-5">
      <h1 className="text-center mb-4">Lista de Imóveis</h1>
      <div className="row">
        {imoveis.map(imovel => (
          <div key={imovel.id} className="col-md-4 mb-4">
            <div className="card">             
              <div className="card-body">
                <h5 className="card-title">{imovel.titulo}</h5>
                <p className="card-text">{imovel.descricao}</p>
                <button className="btn btn-primary" onClick={() => console.log(imovel.id)}>Ver mais</button>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Home;
