<script>
import Layout from "../../layouts/main";
import PageHeader from "@/components/page-header";
import {required} from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import {notifyModel} from "@/models/notifyModel";
import {pagingModel} from "@/models/pagingModel";
import {khoiCoQuanModel} from "@/models/khoiCoQuanModel";
import {CONSTANTS} from "@/helpers/constants";

export default {
  page: {
    title: "Khối cơ quan",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout},
  data() {
    return {
      title: "Khối cơ quan",
      items: [
        {
          text: "Khối cơ quan",
          href: "/khoi-co-quan",
          // active: true,
        },
        {
          text: "Danh sách ",
          active: true,
        }
      ],
      data: [],
      showModal: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      model: khoiCoQuanModel.baseJson(),
      listCoQuan: [],
      listRole: [],
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 10,
      pageOptions: [5, 10, 25, 50, 100],
      filter: null,
      todoFilter: null,
      filterOn: [],
      todofilterOn: [],
      sortBy: "age",
      sortDesc: false,
      fields: [
        {
          key: 'STT',
          label: 'STT',
          class: "cs-text-center",
          thStyle: {width: '50px', minWidth: '50px',},
          thClass: 'hidden-sortable'
        },
        {
          key: "code",
          label: "Code",
          thStyle: {width: '180px', minWidth: '180px'},
        },
        {
          key: "ten",
          label: "Tên",
          thStyle: "text-align:center",
        },
        {
          key: "permissions",
          label: "Số lượng",
          class: "text-center",
          thStyle: {width: '110px', minWidth: '110px'},
          thClass: 'hidden-sortable'
        },
        {
          key: 'process',
          label: 'Xử lý',
          class: "text-center btn-process",
          thStyle: {width: '150px', minWidth: '150px'},
          thClass: 'hidden-sortable'
        }
      ],
      itemFilter:{
        code: null,
        name: null,
      }

    };
  },
  validations: {
    model: {
      ten: {required},
      sort: {required},
      code: {required},
    },
  },
  created() {
  },
  watch: {
    model: {
      deep: true,
      handler(val) {
        // addCoQuanToModel()
        // this.saveValueToLocalStorage()
      }
    },
    showModal(status) {
      if (status == false) this.model = khoiCoQuanModel.baseJson();
    },
    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    },
  },
  methods: {
    clearSearch(){
      this.itemFilter.code= null;
      this.itemFilter.name= null;
    },
    async handleUpdate(id) {
      await this.$store.dispatch("khoiCoQuanStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = khoiCoQuanModel.fromJson(res.data);
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          this.$refs.tblList.refresh()
        }
      });
    },
    async handleDetail(id) {
      await this.$store.dispatch("khoiCoQuanStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = khoiCoQuanModel.fromJson(res.data);
          this.showDetail = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("khoiCoQuanStore/delete", this.model.id).then((res) => {
          if (res.resultCode === 'SUCCESS') {
            this.showDeleteModal = false;
            this.$refs.tblList.refresh()
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          // });
        });
      }
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    async handleSubmit(e) {
      e.preventDefault();
      this.submitted = true;
      this.$v.$touch();
      if (this.$v.$invalid) {
        return;
      } else {
        let loader = this.$loading.show({
          container: this.$refs.formContainer,
        });
        if (
            this.model.id != 0 &&
            this.model.id != null &&
            this.model.id
        ) {
          // Update model
          await this.$store.dispatch("khoiCoQuanStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.$refs.tblList.refresh()
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("khoiCoQuanStore/create", khoiCoQuanModel.toJson(this.model)).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.$refs.tblList.refresh()
              this.model = {};
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }
      this.submitted = false;
    },
    myProvider(ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
      }
      this.loading = true
      try {
        let promise = this.$store.dispatch("khoiCoQuanStore/getPagingParams", params)
        return promise.then(resp => {
          if(resp.resultCode == CONSTANTS.SUCCESS){
            let data = resp.data;
            this.totalRows = data.totalRows
            let items = data.data
            this.numberOfElement = items.length
            this.loading = false
            return items || []
          }else{
            return [];
          }
        })
      } finally {
        this.loading = false
      }
    },
    // todoFiltered(filteredItems) {
    //   // Trigger pagination to update the number of buttons/pages due to filtering
    //   this.todoTotalRows = filteredItems.length;
    //   this.todocurrentPage = 1;
    // }
  }
}
</script>
<template>
  <Layout>
<!--    <PageHeader :title="title" :items="items"/>-->
    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="row">
              <div class="col-md-4 col-12 d-flex align-items-center">
                <h4 class="font-size-18 fw-bold text-dark">Khối cơ quan</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button v-b-toggle.collapseSearch variant="light"
                          class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
                </b-button>
                <b-button type="button" variant="primary" class="w-md" @click="showModal = true" size="sm">
                  <i class="mdi mdi-plus me-1 label-icon"></i> Thêm khối cơ quan
                </b-button>
              </div>
            </div>
            <b-collapse id="collapseSearch" class="mt-1">
              <div class="row">
                <div class="col-12">
                  <div class="d-flex justify-content-between align-items-end flex-wrap mb-2">
                    <!-- Nội dung -->
                    <div class="flex-grow-1 me-2">
                      <label>Mã cơ quan</label>
                      <input
                          size="sm"
                          type="text"
                          name="untyped-input"
                          class="form-control"
                          v-model="itemFilter.code"
                          placeholder="Nhập mã cơ quan.."
                      />
                    </div>
                    <!-- Nội dung -->
                    <div class="flex-grow-1 me-2">
                      <label>Tên cơ quan</label>
                      <input
                          size="sm"
                          type="text"
                          name="untyped-input"
                          class="form-control"
                          v-model="itemFilter.name"
                          placeholder="Nhập tên cơ quan..."
                      />
                    </div>
                    <!--  Xử lý -->
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
            <div class="row mb-2">
              <div class="col-sm-4">
              </div>
              <div class="col-sm-8">
                <div class="text-sm-end">
                  <b-modal
                      v-model="showModal"
                      title="Thông tin khối cơ quan"
                      title-class="text-black font-18"
                      body-class="p-3"
                      hide-footer
                      centered
                      no-close-on-backdrop
                      size="lg"
                  >
                    <form @submit.prevent="handleSubmit"
                          ref="formContainer">
                      <div class="row">
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Code</label>
                            <span style="color: red">&nbsp;*</span>
                            <input
                                id="ten"
                                v-model.trim="model.code"
                                type="text"
                                class="form-control"
                                placeholder="Nhập mã code"
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.code.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.code.required"
                                class="invalid-feedback"
                            >
                              Tên khối cơ quan không được để trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Tên khối cơ quan</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="ten"
                                v-model.trim="model.ten"
                                type="text"
                                class="form-control"
                                placeholder="Nhập tên nhóm"
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.ten.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.ten.required"
                                class="invalid-feedback"
                            >
                              Tên khối cơ quan không được để trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Nhập số thứ tự</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="thuTu"
                                v-model="model.sort"
                                type="number"
                                min="0"
                                oninput="validity.valid||(value='');"
                                class="form-control"
                                placeholder="Nhập thứ tự"
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.sort.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.sort.required"
                                class="invalid-feedback"
                            >
                              Thứ tự không được để trống.
                            </div>
                          </div>
                        </div>
                      </div>
                      <div class="text-end pt-2 mt-3">
                        <b-button variant="light" size="sm" class="w-md" @click="showModal = false">
                          Đóng
                        </b-button>
                        <b-button type="submit" size="sm" variant="primary" class="ms-1 w-md">
                          Lưu
                        </b-button>
                      </div>
                    </form>
                  </b-modal>
                  <b-modal
                      v-model="showDetail"
                      title="Thông tin chi tiết khối cơ quan"
                      title-class="text-black font-18"
                      body-class="p-3"
                      hide-footer
                      centered
                      no-close-on-backdrop
                      size="lg"
                  >
                    <form @submit.prevent="handleSubmit"
                          ref="formContainer">
                      <div class="row">
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Tên nhóm : </label>
                            <input
                                v-model="model.name"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Thứ tự : </label>
                            <input
                                v-model="model.sort"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Người tạo : </label>
                            <input
                                v-model="model.createdBy"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Ngày tạo: </label>
                            <input
                                v-model="model.createdAt"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Người cập nhật : </label>
                            <input
                                v-model="model.modifiedBy"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Ngày cập nhật : </label>
                            <input
                                v-model="model.modifiedAt"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                      </div>
                      <div class="text-end pt-2 mt-3">
                        <b-button variant="light" @click="showDetail = false">
                          Đóng
                        </b-button>
                      </div>
                    </form>
                  </b-modal>
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
                <div class="table-responsive-sm">
                  <b-table
                      class="datatables custom-table"
                      :items="myProvider"
                      :fields="fields"
                      striped
                      bordered
                      responsive="sm"
                      :per-page="perPage"
                      :current-page="currentPage"
                      :sort-by.sync="sortBy"
                      :sort-desc.sync="sortDesc"
                      :filter="filter"
                      :filter-included-fields="filterOn"
                      ref="tblList"
                      primary-key="id"
                      tbody-tr-class="b-table-khoicoquan"
                  >
                    <template v-slot:cell(STT)="data">
                      {{ data.index + ((currentPage - 1) * perPage) + 1 }}
                    </template>
                    <template v-slot:cell(name)="row">&nbsp;&nbsp;
                      <div style="text-align: left ; margin-top: -30px ; margin-left: 30px">
                        {{ row.value }}
                      </div>
                    </template>
<!--                    <template v-slot:cell(permissions)="data">-->
<!--                      <router-link :to='`/nhom-quyen/action/${data.item.id}`'>-->
<!--                        <b-button-->
<!--                            v-if="data.item.permissions.length > 0 "-->
<!--                            variant="outline-success btn-sm">{{ data.item.permissions.length }}-->
<!--                        </b-button>-->
<!--                        <b-button v-else  variant="outline-success btn-sm">-->
<!--                          {{ 0 }}-->
<!--                        </b-button>-->
<!--                      </router-link>-->
<!--                    </template>-->
                    <template v-slot:cell(process)="data">
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-detail btn-sm"
                          data-toggle="tooltip" data-placement="bottom" title="Chi tiết"
                          v-on:click="handleDetail(data.item.id)">
                        <i class="fas fa-eye "></i>
                      </button>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-edit btn-sm"
                          data-toggle="tooltip" data-placement="bottom" title="Cập nhật"
                          v-on:click="handleUpdate(data.item.id)">
                        <i class="fas fa-pencil-alt"></i>
                      </button>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-delete btn-sm"
                          data-toggle="tooltip" data-placement="bottom" title="Xóa"
                          v-on:click="handleShowDeleteModal(data.item.id)">
                        <i class="fas fa-trash-alt"></i>
                      </button>
                    </template>

                  </b-table>
                </div>
                <div class="row">
                  <b-col>
                    <div>Hiển thị {{ numberOfElement }} trên tổng số {{ totalRows }} dòng</div>
                  </b-col>
                  <div class="col">
                    <div
                        class="dataTables_paginate paging_simple_numbers float-end">
                      <ul class="pagination pagination-rounded mb-0">
                        <!-- pagination -->
                        <b-pagination
                            v-model="currentPage"
                            :total-rows="totalRows"
                            :per-page="perPage"
                        ></b-pagination>
                      </ul>
                    </div>
                  </div>
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
  </Layout>
</template>
<style>
.hidden-sortable:after, .hidden-sortable:before {
  display: none !important;
}

.table > tbody > tr > td {
  padding: 0px;
  line-height: 30px;
}

@media only screen and (max-width: 768px) {
  .b-table-khoicoquan>td:nth-of-type(1):before{
    content: "STT";
    font-weight: bold;
    color: #00568c;
  }
  .b-table-khoicoquan>td:nth-of-type(2):before {
    content: "Code";
    font-weight: bold;
    color: #00568c;
  }
  .b-table-khoicoquan>td:nth-of-type(3):before {
    content: "Tên";
    font-weight: bold;
    color: #00568c;
  }
  .b-table-khoicoquan>td:nth-of-type(4):before {
    content: "Số lượng";
    font-weight: bold;
    color: #00568c;
  }
  .b-table-khoicoquan>td:nth-of-type(5):before {
    content: "";
    font-weight: bold;
    color: #00568c;
  }
}
</style>
