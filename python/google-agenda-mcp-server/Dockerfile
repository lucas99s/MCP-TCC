FROM python:3.11-slim
WORKDIR /app

# Copy requirements and install
COPY requirements.docker.txt .
RUN apt-get update \
	 && apt-get install -y --no-install-recommends \
		 build-essential \
		 libssl-dev \
		 libffi-dev \
		 python3-dev \
		 cargo \
		 pkg-config \
		 git \
	 && python -m pip install --upgrade pip setuptools wheel \
	 && python -m pip install --no-cache-dir -r requirements.docker.txt \
	 && rm -rf /var/lib/apt/lists/*

# Copy app sources
COPY . .

ENV PYTHONUNBUFFERED=1
EXPOSE 3333

CMD ["python", "server.py"]
