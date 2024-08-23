# Use an official Node.js image as a base
FROM node:20-alpine

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and package-lock.json to the working directory
COPY Web/Wasteless/ClientApp/package*.json ./

# Install dependencies
RUN npm install --legacy-peer-deps

# Copy the rest of the application code to the working directory
COPY Web/Wasteless/ClientApp/ ./

# Build the React app
RUN npm run build

# Install 'serve' to serve the built application
RUN npm install -g serve

# Expose the port defined by the environment variable
EXPOSE $PORT

# Command to start the app, ensuring it uses the correct port
CMD ["serve", "-s", "build", "-l", "tcp://0.0.0.0:$PORT"]
