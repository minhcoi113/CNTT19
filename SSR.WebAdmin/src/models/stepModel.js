const toJson = (item) => {
    return {
        id: item.id,
        color: item.color,
        name: item.name,
        description: item.description,
    }
}

const fromJson = (item) => {
    return {
        id: item.id,
        color: item.color,
        name: item.name,
        description: item.description,
    }
}

const baseJson = () => {
    return {
        id: null,
        color: null,
        name: null,
        description: null,
    }
}

const toListModel = (items) =>{
    if(items.length > 0){
        let data = [];
        items.map((value, index) =>{
            data.push(fromJson(value));
        })
        return data??[];
    }
    return [];
}
export const stepModel = {
    toJson, fromJson, baseJson, toListModel
}
