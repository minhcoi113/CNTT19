<script>
import Layout from "../../layouts/main";
import appConfig from "@/app.config";
import {projectModel} from "@/models/projectModel";
import {required} from "vuelidate/lib/validators";
import urlSlug from 'url-slug'
import {notifyModel} from "@/models/notifyModel";
import {pagingModel} from "@/models/pagingModel";
import {CONSTANTS} from "@/helpers/constants";
import {labelModel} from "@/models/labelModel";
import {userModel} from "@/models/userModel";
import {groupModel} from "@/models/groupModel";
export default {
  page: {
    title: "Quản lý dự án",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout},
  data() {
    return {
      title: "Quản lý dự án",
      data: [],
      showModal: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 10,
      pageOptions: [5,10, 25, 50, 100],
      filter: null,
      filterOn: [],
      isBusy: false,
      sortBy: "age",
      sortDesc: false,
      model: projectModel.baseJson(),
      itemFilter:{
        code: null,
        name: null,
      },
      items: [
        {
          text: "Dự án",
          href:"/du-an",
          // active: true,
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],
      
      fields: [
      { key: 'STT',
          label: 'STT',
          class: 'cs-text-center',
          sortable: false,
          thClass: 'hidden-sortable',
          thStyle: {width: '40px', minWidth: '40px'}
        },
        {
          key: "name",
          label: "Tên dự án",
          sortable: true,
          thClass: 'hidden-sortable',
          thStyle: {width: '400px', minWidth: '100px'},
        },
        {
          key: "description",
          label: "Mô tả",
          // class: 'td-xuly',
          thClass: 'hidden-sortable',
          sortable: true,
          // thStyle: {width: '100px', minWidth: '100px'},
        },
        // {
        //   key: "label",
        //   label: "label",
        //   class: 'td-xuly',
        //   sortable: true,
        //   thStyle: {width: '120px', minWidth: '120px'},
        // },
       
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly btn-process',
          thClass: 'hidden-sortable',
          sortable: false,
          thStyle: {width: '130px', minWidth: '130px'},
        }
      ],
       
      optionsUser: [],
      optionsGroup: [],
      optionsLabel: [],
      
    };
  },
  async created() {
    this.getUser();
    this.getGroup();
    this.getLabel();
    if(this.$route.params.id){
      this.getById(this.$route.params.id);
    }else{
      this.model = projectModel.baseJson();
    }
    if(this.$route.params.slug){
      this.getBySlug(this.$route.params.slug);
    }else{
      this.model = projectModel.baseJson();
    }


    },
  watch:{
    model: {
      
    }
  },
  computed:{
     
  },
  mounted() {
  },
  validations: {
    model: {
    
      name: {required},
      description: {required},
      // member: {required},
      // group: {required},
      label: {required},
      slug: {required},
    
    },
  },
  
  methods: {
    clearSearch(){
      this.itemFilter.code= null;
      this.itemFilter.name= null;
      
      this.filter = null;
    
    },
    handleSearch() {
      this.fnGetList()
    },
    async fnGetList() {
         this.$refs.tblList?.refresh()
    },
     
    
    async getUser(){
      await this.$store.dispatch("userStore/getAll").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsUser = res.data;
          return;
        }
        this.optionsUser = [];
      });
    },

    async getGroup(){
      await this.$store.dispatch("groupStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsGroup = res.data;
          return;
        }
        this.optionsGroup = [];
      });
    },
    async getLabel(){
      await this.$store.dispatch("labelStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsLabel = res.data;
          return;
        }
        this.optionsLabel = [];
      });
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("projectStore/delete", this.model.id).then((res) => {
          if (res.resultCode==='SUCCESS') {
            this.fnGetList();
            this.showDeleteModal = false;
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        });
      }
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    async handleUpdate(slug) {
      await this.$store.dispatch("projectStore/getBySlug", slug).then((res) => {
        this.$router.push("")
        if (res.resultCode==='SUCCESS') {
          this.model = projectModel.toJson(res.data);
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    myProvider (ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
      }
      this.loading = true
      try {
        let promise =  this.$store.dispatch("projectStore/getPagingParams", params)
        return promise.then(resp => {
          let items = resp.data.data
          this.totalRows = resp.data.totalRows
          this.numberOfElement = resp.data.data.length
          this.loading = false
          return items || []
        })
      } finally {
        this.loading = false
      }
    },
    
    base64ToArrayBuffer(base64) {
      var binaryString = window.atob(base64);
      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
      }
      return bytes;
    },
    handleNewPost(){
      this.$router.push("/tao-project")
    },
    handleDetail(id){
      this.$router.push("/du-an/chi-tiet/" + id)
    },
    Project(slug){
      this.$router.push(slug + "/danh-sach-yeu-cau-loi")
    },
    FormatJSon(FilterData, JsonData){
      return JsonData.map((v)=>FilterData.map((j=>{
        return v[j];
      })))
    },
    handleDetailProject(slug){

      let currentProject = JSON.stringify(slug)

      localStorage.setItem("currentProject", currentProject);

      this.$router.push(`/${slug}/danh-sach-yeu-cau-loi`)
    }
  },
};
</script>

<template>
  <Layout>
<!--    <PageHeader/>-->
    <div class="row">
    <div class="col-12">
      <div class="card mb-2">
        <div class="card-body">
          <div class="row">
            <div class="col-md-4 col-12 d-flex align-items-center">
              <h4 class="font-size-18 fw-bold cs-title-page">Danh sách dự án</h4>
            </div>
            <div class="col-md-8 col-12 text-end">
              <b-button v-b-toggle.collapseSearch variant="light"
                          class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
                </b-button>
              <b-button
                    variant="primary"
                    type="button"
                    class="btn w-md btn-primary"
                    @click="handleNewPost"
                    size="sm"
                >
                  <i class="mdi mdi-plus me-1"></i> Thêm dự án
                </b-button>
            </div>
          </div>
          <b-collapse id="collapseSearch" class="mt-1">
              <div class="row">
                <div class="col-12">
                  <div class="d-flex justify-content-between align-items-end flex-wrap mb-2">
                    <!-- Nội dung -->
                    <div class="flex-grow-1 me-2">
                      <label>Nội dung</label>
                      <input
                          v-model = "filter"
                          type="text"
                          class="form-control"
                          placeholder="Tìm kiếm ..."
                      />
                      <i class="bx bx-search-alt search-icon"></i>
                    </div>
                    <div class="flex-grow-0 ms-2">
                      <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="handleSearch" variant="light"
                                    class="btn w-md btn-primary me-2" size="md">
                            <i class="fas fa-search align-middle me-2"></i>
                            Tìm kiếm
                          </b-button>
                        </div>
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="clearSearch" variant="light"
                                    class="btn w-md btn-secondary me-2" size="md">
                            <i class="fas fa-redo-alt align-middle me-2"></i>
                            Làm mới
                          </b-button>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </b-collapse>
        </div>
      </div>
    </div>
  </div>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body">
            <div class="col-sm-4">
                <div class="search-box me-2 mb-2 d-inline-block">
                  <div class="position-relative">
                    <input
                        v-model = "filter"
                        type="text"
                        class="form-control"
                        placeholder="Tìm kiếm ..."
                    />
                    <i class="bx bx-search-alt search-icon"></i>
                  </div>
                </div>
              </div>
            <div class="row">
              <div class="col-12">
                <div class="row mb-3">
                  <div class="col-sm-12 col-md-6">
                    <div id="tickets-table_length" class="dataTables_length">
                      <label class="d-inline-flex align-items-center">
                        Hiện
                        <b-form-select
                            class="form-select form-select-sm"
                            v-model="perPage"
                            size="sm"
                            :options="pageOptions"
                        ></b-form-select
                        >&nbsp;dòng
                      </label>
                    </div>
                  </div>
                </div>
                <div class="table-responsive mb-0">
                    <b-table
                      class="datatables custom-table"
                      :items="myProvider"
                      :fields="fields"
                      responsive="sm"
                      :per-page="perPage"
                      :current-page="currentPage"
                      :sort-by.sync="sortBy"
                      :sort-desc.sync="sortDesc"
                      :filter="filter"
                      :filter-included-fields="filterOn"
                      ref="tblList"
                      primary-key="id"
                      :busy.sync="isBusy"
                      tbody-tr-class="b-table-chucvu"
                  >                 
                    <template v-slot:cell(STT)="data">
                      {{ data.index + ((currentPage-1)*perPage) + 1  }}
                    </template>
                    
                    
                    <template v-slot:cell(name)="data">&nbsp;&nbsp;
                      <a class="link-dark" href="" v-on:click="handleDetailProject(data.item.slug)">
                        {{data.item.name}}  
                      </a>
                    </template>
                    <template v-slot:cell(process)="data">
                       <router-link :to='`/du-an/chi-tiet/${data.item.slug}`'>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-edit btn-sm"
                          v-on:click="handleDetail(data.item.slug)"
                          >
                        <i class="fas fa-pencil-alt"></i>
                      </button>
                    </router-link>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-delete btn-sm"
                          v-on:click="handleShowDeleteModal(data.item.id)">
                        <i class="fas fa-trash-alt"></i>
                      </button>
                    </template>
                  </b-table>
                  <template v-if="isBusy">
                    <div align="center">Đang tải dữ liệu</div>
                  </template>
                  <template v-if="totalRows <= 0 && !isBusy">
                    <div align="center">Không có dữ liệu</div>
                  </template>
                </div>
                <div class="row">
                  <b-col>
                    <div>Hiển thị {{numberOfElement}} trên tổng số {{totalRows}} dòng</div>
                  </b-col>
                  <div class="col">
                    <div
                        class="dataTables_paginate paging_simple_numbers float-end">
                      <ul class="pagination pagination-rounded mb-0">
                        <!-- pagination -->
                        <b-pagination
                            class="pagination-rounded"
                            v-model="currentPage"
                            :total-rows="totalRows"
                            :per-page="perPage"
                            size="sm"
                        ></b-pagination>
                      </ul>
                    </div>
                  </div>
                </div>

              </div>
            </div>
            <b-modal
                v-model="showDeleteModal"
                centered
                title="Xóa dữ liệu"
                title-class="font-18"
                no-close-on-backdrop
            >
              <p>
                Dữ liệu xóa sẽ không được phục hồi!
              </p>
              <template #modal-footer>
                <button v-b-modal.modal-close_visit
                        class="btn btn-outline-info m-1"
                        v-on:click="showDeleteModal = false">
                  Đóng
                </button>
                <button v-b-modal.modal-close_visit
                        class="btn btn-danger btn m-1"
                        v-on:click="handleDelete">
                  Xóa
                </button>
              </template>
            </b-modal>
          </div>
        </div>
      </div>
    </div>
  </Layout>
</template>
<style >
.td-stt {
  text-align: center;
  width: 55px;
}
.td-username {
  text-align: center;
  width: 120px;
}
.td-ten {
  text-align: center;
  width: 140px;
}
.td-email {
  text-align: center;
  width: 120px;
}
.td-donVi{
  text-align: center;
  width: 180px;
}
.td-xuly {
  text-align: center;
  width: 80px;
}
.td-sodienthoai {
  text-align: center;
  width: 100px;
}

</style>
<style lang="scss">
#my-strictly-unique-vue-upload-multiple-image {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
</style>