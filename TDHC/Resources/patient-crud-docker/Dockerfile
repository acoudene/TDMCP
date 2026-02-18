FROM node:18-alpine

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm install --production

# Copy application files
COPY server-simple.js ./server.js
COPY index.html ./
COPY patients-data.json ./

EXPOSE 9999

CMD ["node", "server.js"]
