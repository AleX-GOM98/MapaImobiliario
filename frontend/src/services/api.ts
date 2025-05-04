// src/services/api.ts
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5214/api/', // Ajuste a URL para o seu backend
});

export default api;

// Adicione esta exportação vazia para garantir que o arquivo seja tratado como um módulo
export {};
