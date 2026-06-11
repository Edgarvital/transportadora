# Estágio 1: Build da aplicação
FROM node:20-alpine AS build
WORKDIR /app

# Copia os arquivos de dependência primeiro (aproveita o cache do Docker)
COPY package*.json ./
RUN npm install

# Copia o restante do código e gera o build de produção
COPY . .
# Nota: Se estiver usando Vite, o comando padrão gera a pasta 'dist'. Se for Create-React-App, gera 'build'.
RUN npm run build 

# Estágio 2: Servidor Nginx
FROM nginx:alpine
# Copia os arquivos estáticos gerados no estágio anterior (assumindo Vite: /app/dist)
COPY --from=build /app/dist /usr/share/nginx/html

# Expõe a porta 80 do contêiner
EXPOSE 80

# Inicia o Nginx
CMD ["nginx", "-g", "daemon off;"]